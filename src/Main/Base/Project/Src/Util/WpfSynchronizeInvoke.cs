// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Implements the ISynchronizeInvoke interface by using a WPF dispatcher
	/// to perform the cross-thread call.
	/// </summary>
	sealed class WpfSynchronizeInvoke : ISynchronizeInvoke
	{
		readonly Dispatcher dispatcher;
		
		public WpfSynchronizeInvoke(Dispatcher dispatcher)
		{
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");
			this.dispatcher = dispatcher;
		}
		
		public bool InvokeRequired {
			get {
				return !dispatcher.CheckAccess();
			}
		}
		
		public IAsyncResult BeginInvoke(Delegate method, object[] args)
		{
			DispatcherOperation op;
			if (args == null || args.Length == 0)
				op = dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
			else if (args.Length == 1)
				op = dispatcher.BeginInvoke(DispatcherPriority.Normal, method, args[0]);
			else
				op = dispatcher.BeginInvoke(DispatcherPriority.Normal, method, args[0], args.Splice(1));
			return new AsyncResult(op);
		}
		
		sealed class AsyncResult : IAsyncResult
		{
			internal readonly DispatcherOperation op;
			readonly object lockObj = new object();
			ManualResetEvent resetEvent;
			
			public AsyncResult(DispatcherOperation op)
			{
				this.op = op;
			}
			
			public bool IsCompleted {
				get {
					return op.Status == DispatcherOperationStatus.Completed;
				}
			}
			
			public WaitHandle AsyncWaitHandle {
				get {
					lock (lockObj) {
						if (resetEvent == null) {
							op.Completed += op_Completed;
							resetEvent = new ManualResetEvent(false);
							if (IsCompleted)
								resetEvent.Set();
						}
						return resetEvent;
					}
				}
			}

			void op_Completed(object sender, EventArgs e)
			{
				lock (lockObj) {
					resetEvent.Set();
				}
			}
			
			public object AsyncState {
				get { return null; }
			}
			
			public bool CompletedSynchronously {
				get { return false; }
			}
		}
		
		public object EndInvoke(IAsyncResult result)
		{
			AsyncResult r = result as AsyncResult;
			if (r == null)
				throw new ArgumentException("result must be the return value of a WpfSynchronizeInvoke.BeginInvoke call!");
			r.op.Wait();
			return r.op.Result;
		}
		
		public object Invoke(Delegate method, object[] args)
		{
			if (args.Length == 0)
				return dispatcher.Invoke(DispatcherPriority.Normal, method);
			else if (args.Length == 1)
				return dispatcher.Invoke(DispatcherPriority.Normal, method, args[0]);
			else
				return dispatcher.Invoke(DispatcherPriority.Normal, method, args[0], args.Splice(1));
		}
	}
}
