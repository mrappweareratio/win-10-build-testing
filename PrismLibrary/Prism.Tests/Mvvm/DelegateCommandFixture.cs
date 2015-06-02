// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Commands;
using System.Threading.Tasks;
using Prism.Tests.Mocks.Commands;

namespace Prism.Tests.Mvvm
{
    /// <summary>
    /// Summary description for DelegateCommandFixture
    /// </summary>
    [TestClass]
    public class DelegateCommandFixture
    {
        [TestMethod]
        public void WhenConstructedWithGenericTypeOfObject_InitializesValues()
        {
            // Prepare

            // Act
            var actual = new DelegateCommand<object>(param => { });

            // verify
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void WhenConstructedWithGenericTypeOfNullable_InitializesValues()
        {
            // Prepare

            // Act
            var actual = new DelegateCommand<int?>(param => { });

            // verify
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void WhenConstructedWithGenericTypeIsNonNullableValueType_Throws()
        {
            // Prepare

            // Act
            var actual = new DelegateCommand<int>(param => { });

            // verify
        }

        [TestMethod]
        public async Task ExecuteCallsPassedInExecuteDelegate()
        {
            var handlers = new DelegateHandlers();
            var command = new DelegateCommand<object>(handlers.Execute);
            object parameter = new object();

            await command.Execute(parameter);

            Assert.AreSame(parameter, handlers.ExecuteParameter);
        }

        [TestMethod]
        public void CanExecuteCallsPassedInCanExecuteDelegate()
        {
            var handlers = new DelegateHandlers();
            var command = new DelegateCommand<object>(handlers.Execute, handlers.CanExecute);
            object parameter = new object();

            handlers.CanExecuteReturnValue = true;
            bool retVal = command.CanExecute(parameter);

            Assert.AreSame(parameter, handlers.CanExecuteParameter);
            Assert.AreEqual(handlers.CanExecuteReturnValue, retVal);
        }

        [TestMethod]
        public void CanExecuteReturnsTrueWithouthCanExecuteDelegate()
        {
            var handlers = new DelegateHandlers();
            var command = new DelegateCommand<object>(handlers.Execute);

            bool retVal = command.CanExecute(null);

            Assert.AreEqual(true, retVal);
        }


        [TestMethod]
        public void RaiseCanExecuteChangedRaisesCanExecuteChanged()
        {
            var handlers = new DelegateHandlers();
            var command = new DelegateCommand<object>(handlers.Execute);
            bool canExecuteChangedRaised = false;
            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            command.RaiseCanExecuteChanged();

            Assert.IsTrue(canExecuteChangedRaised);
        }

        [TestMethod]
        public void CanRemoveCanExecuteChangedHandler()
        {
            var command = new DelegateCommand<object>((o) => { });

            bool canExecuteChangedRaised = false;

            EventHandler handler = (s, e) => canExecuteChangedRaised = true;

            command.CanExecuteChanged += handler;
            command.CanExecuteChanged -= handler;
            command.RaiseCanExecuteChanged();

            Assert.IsFalse(canExecuteChangedRaised);
        }

        [TestMethod]
        public void ShouldPassParameterInstanceOnExecute()
        {
            bool executeCalled = false;
            MyClass testClass = new MyClass();
            ICommand command = new DelegateCommand<MyClass>(delegate(MyClass parameter)
            {
                Assert.AreSame(testClass, parameter);
                executeCalled = true;
            });

            command.Execute(testClass);
            Assert.IsTrue(executeCalled);
        }

        [TestMethod]
        public void ShouldPassParameterInstanceOnCanExecute()
        {
            bool canExecuteCalled = false;
            MyClass testClass = new MyClass();
            ICommand command = new DelegateCommand<MyClass>((p) => { }, delegate(MyClass parameter)
            {
                Assert.AreSame(testClass, parameter);
                canExecuteCalled = true;
                return true;
            });

            command.CanExecute(testClass);
            Assert.IsTrue(canExecuteCalled);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfAllDelegatesAreNull()
        {
            var command = new DelegateCommand<object>(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfExecuteMethodDelegateNull()
        {
            var command = new DelegateCommand<object>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfCanExecuteMethodDelegateNull()
        {
            var command = new DelegateCommand<object>((o) => { }, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DelegateCommandBaseShouldThrowIfAllDelegatesAreNull()
        {
            var command = new DelegateCommandMock(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DelegateCommandBaseShouldThrowIfExecuteMethodDelegateNull()
        {
            var command = new DelegateCommandMock(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DelegateCommandBaseShouldThrowIfCanExecuteMethodDelegateNull()
        {
            var command = new DelegateCommandMock((o) => { }, null);
        }

        [TestMethod]
        public void NonGenericDelegateCommandShouldInvokeExplicitExecuteFunc()
        {
            bool executed = false;
            ICommand command = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => { executed = true; }));
            command.Execute(null);
            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void IsActivePropertyIsFalseByDeafult()
        {
            var command = new DelegateCommand<object>(DoNothing);
            Assert.IsFalse(command.IsActive);
        }

        [TestMethod]
        public void IsActivePropertyChangeFiresEvent()
        {
            bool fired = false;
            var command = new DelegateCommand<object>(DoNothing);
            command.IsActiveChanged += delegate { fired = true; };
            command.IsActive = true;

            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void ShouldKeepWeakReferenceToOnCanExecuteChangedHandlers()
        {
            var command = new DelegateCommand<MyClass>((MyClass c) => { });

            var handlers = new CanExecutChangeHandler();
            var weakHandlerRef = new WeakReference(handlers);
            command.CanExecuteChanged += handlers.CanExecuteChangeHandler;
            handlers = null;

            GC.Collect();
            command.RaiseCanExecuteChanged();

            Assert.IsFalse(weakHandlerRef.IsAlive);
            Assert.IsNotNull(command); // Only here to ensure command survives optimizations and the GC.Collect
        }

        [TestMethod]
        public async Task NonGenericDelegateCommandExecuteShouldInvokeExecuteAction()
        {
            bool executed = false;
            var command = new DelegateCommand(() => { executed = true; });
            await command.Execute();

            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void NonGenericDelegateCommandCanExecuteShouldInvokeCanExecuteFunc()
        {
            bool invoked = false;
            var command = new DelegateCommand(() => { }, () => { invoked = true; return true; });

            bool canExecute = command.CanExecute();

            Assert.IsTrue(invoked);
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        public void NonGenericDelegateCommandShouldDefaultCanExecuteToTrue()
        {
            var command = new DelegateCommand(() => { });
            Assert.IsTrue(command.CanExecute());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NonGenericDelegateThrowsIfDelegatesAreNull()
        {
            var command = new DelegateCommand(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NonGenericDelegateCommandThrowsIfExecuteDelegateIsNull()
        {
            var command = new DelegateCommand(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NonGenericDelegateCommandThrowsIfCanExecuteDelegateIsNull()
        {
            var command = new DelegateCommand(() => { }, null);
        }

        [TestMethod]
        public void GenericDelegateCommandFromAsyncHandlerWithExecuteFuncShouldNotBeNull()
        {
            var command = DelegateCommand<object>.FromAsyncHandler(async (o) => await Task.Run(() => { }));
            Assert.IsNotNull(command);
        }

        [TestMethod]
        public void GenericDelegateCommandFromAsyncHandlerWithExecuteAndCanExecuteFuncShouldNotBeNull()
        {
            var command = DelegateCommand<object>.FromAsyncHandler(async (o) => await Task.Run(() => { }), (o) => true);
            Assert.IsNotNull(command);
        }

        [TestMethod]
        public void GenericDelegateCommandFromAsyncHandlerCanExecuteShouldBeTrueByDefault()
        {
            var command = DelegateCommand<object>.FromAsyncHandler(async (o) => await Task.Run(() => { }));
            var canExecute = command.CanExecute(null);
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GenericDelegateCommandFromAsyncHandlerWithNullExecuteFuncShouldThrow()
        {
            var command = DelegateCommand<object>.FromAsyncHandler(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GenericDelegateCommandFromAsyncHandlerWithNullCanExecuteFuncShouldThrow()
        {
            var command = DelegateCommand<object>.FromAsyncHandler(async (o) => await Task.Run(() => { }), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DelegateCommandBaseWithNullExecuteFuncShouldThrow()
        {
            var command = DelegateCommandMock.FromAsyncHandler(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DelegateCommandBaseWithNullCanExecuteFuncShouldThrow()
        {
            var command = DelegateCommand<object>.FromAsyncHandler(async (o) => await Task.Run(() => { }), null);
        }

        [TestMethod]
        public async Task GenericDelegateCommandFromAsyncHandlerExecuteShouldInvokeExecuteFunc()
        {
            bool executed = false;

            var command = DelegateCommand<object>.FromAsyncHandler(async (o) => await Task.Run(() => executed = true));
            await command.Execute(null);

            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void DelegateCommandFromAsyncHandlerWithExecuteFuncShouldNotBeNull()
        {
            var command = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => { }));
            Assert.IsNotNull(command);
        }

        [TestMethod]
        public void DelegateCommandFromAsyncHandlerCanExecuteShouldBeTrueByDefault()
        {
            var command = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => { }));
            var canExecute = command.CanExecute();
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DelegateCommandFromAsyncHandlerWithNullExecuteFuncShouldThrow()
        {
            var command = DelegateCommand.FromAsyncHandler(null);
        }

        [TestMethod]
        public void DelegateCommandFromAsyncHandlerWithExecuteAndCanExecuteFuncShouldNotBeNull()
        {
            var command = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => { }), () => true);
            Assert.IsNotNull(command);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DelegateCommandFromAsyncHandlerWithNullCanExecuteFuncShouldThrow()
        {
            var command = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => { }), null);
        }

        [TestMethod]
        public async Task DelegateCommandFromAsyncHandlerExecuteShouldInvokeExecuteFunc()
        {
            bool executed = false;

            var command = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => executed = true));
            await command.Execute();

            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void DelegateCommandFromAsyncHandlerCanExecuteShouldInvokeCanExecuteFunc()
        {
            var command = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => { }), () => true);
            var canExecute = command.CanExecute();
            Assert.IsTrue(canExecute);
        }

        class CanExecutChangeHandler
        {
            public bool CanExeucteChangedHandlerCalled;
            public void CanExecuteChangeHandler(object sender, EventArgs e)
            {
                CanExeucteChangedHandlerCalled = true;
            }
        }

        public void DoNothing(object param)
        { }


        class DelegateHandlers
        {
            public bool CanExecuteReturnValue = true;
            public object CanExecuteParameter;
            public object ExecuteParameter;

            public bool CanExecute(object parameter)
            {
                CanExecuteParameter = parameter;
                return CanExecuteReturnValue;
            }

            public void Execute(object parameter)
            {
                ExecuteParameter = parameter;
            }
        }
    }

    internal class MyClass
    {
    }
}
