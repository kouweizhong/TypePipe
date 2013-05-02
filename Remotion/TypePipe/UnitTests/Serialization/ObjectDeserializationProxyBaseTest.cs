// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.Serialization;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.Serialization
{
  [TestFixture]
  public class ObjectDeserializationProxyBaseTest
  {
    private SerializationInfo _info;
    private StreamingContext _context;

    private ObjectDeserializationProxyBase _objectDeserializationProxyBase;

    private IPipelineRegistry _pipelineRegistryMock;
    private Func<IPipeline, AssembledTypeID, StreamingContext, object> _createRealObjectAssertions;

    [SetUp]
    public void SetUp ()
    {
      var serializableType = ReflectionObjectMother.GetSomeSerializableType();
      var formatterConverter = new FormatterConverter();
      _info = new SerializationInfo (serializableType, formatterConverter);
      _context = new StreamingContext ((StreamingContextStates) 7);

      _pipelineRegistryMock = MockRepository.GenerateStrictMock<IPipelineRegistry>();
      _createRealObjectAssertions = (f, t, c) => { throw new Exception ("Setup assertions and return real object."); };

      using (new ServiceLocatorScope (typeof (IPipelineRegistry), () => _pipelineRegistryMock))
      {
        // Use testable class instead of partial mock, because RhinoMocks chokes on non-virtual ISerializable.GetObjectData.
        _objectDeserializationProxyBase = new TestableObjectDeserializationProxyBase (_info, _context, (f, t, c) => _createRealObjectAssertions (f, t, c));
      }
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_objectDeserializationProxyBase.SerializationInfo, Is.SameAs (_info));
    }

    [Test]
    public void GetRealObject ()
    {
      var requestedType = ReflectionObjectMother.GetSomeType();
      var context = new StreamingContext ((StreamingContextStates) 8);

      _info.AddValue ("<tp>requestedType", requestedType.AssemblyQualifiedName);
      _info.AddValue ("<tp>participantConfigurationID", "config1");

      var fakeObjectFactory = MockRepository.GenerateStub<IPipeline>();
      var fakeInstance = MockRepository.GenerateStrictMock<IDeserializationCallback>();
      _pipelineRegistryMock.Expect (mock => mock.Get ("config1")).Return (fakeObjectFactory);
      _createRealObjectAssertions = (factory, type, ctx) =>
      {
        Assert.That (factory, Is.SameAs (fakeObjectFactory));
        Assert.That (type, Is.SameAs (requestedType));
        Assert.That (ctx, Is.EqualTo (context).And.Not.EqualTo (_context));

        return fakeInstance;
      };

      var result = _objectDeserializationProxyBase.GetRealObject (context);

      _pipelineRegistryMock.VerifyAllExpectations();
      fakeInstance.AssertWasNotCalled (mock => mock.OnDeserialization (Arg<object>.Is.Anything));
      Assert.That (result, Is.SameAs (fakeInstance));
      Assert.That (PrivateInvoke.GetNonPublicField (_objectDeserializationProxyBase, "_instance"), Is.SameAs (fakeInstance));
    }

    [Test]
    public void GetRealObject_Caches ()
    {
      var instance = new object ();
      PrivateInvoke.SetNonPublicField (_objectDeserializationProxyBase, "_instance", instance);

      var result = _objectDeserializationProxyBase.GetRealObject (_context);

      Assert.That (result, Is.SameAs (instance));
    }

    [Test]
    public void CreateRealObject ()
    {
      var instance = new object();
      PrivateInvoke.SetNonPublicField (_objectDeserializationProxyBase, "_instance", instance);
      var sender = new object();

      Assert.That (() => _objectDeserializationProxyBase.OnDeserialization (sender), Throws.Nothing);
    }

    [Test]
    public void CreateRealObject_DeserializationCallback ()
    {
      var deserializationCallbackMock = MockRepository.GenerateStrictMock<IDeserializationCallback> ();
      var sender = new object ();
      deserializationCallbackMock.Expect (x => x.OnDeserialization (sender));
      PrivateInvoke.SetNonPublicField (_objectDeserializationProxyBase, "_instance", deserializationCallbackMock);

      _objectDeserializationProxyBase.OnDeserialization (sender);

      deserializationCallbackMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (TypeLoadException), MatchType = MessageMatch.StartsWith,
        ExpectedMessage = "Could not load type 'UnknownType' from assembly 'Remotion.TypePipe, ")]
    public void GetRealObject_UnderlyingTypeNotFound ()
    {
      _info.AddValue ("<tp>requestedType", "UnknownType");
      _info.AddValue ("<tp>participantConfigurationID", "factory1");

      _objectDeserializationProxyBase.GetRealObject (new StreamingContext());
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "This method should not be called.")]
    public void GetObjectData ()
    {
      _objectDeserializationProxyBase.GetObjectData (null, new StreamingContext());
    }
  }
}