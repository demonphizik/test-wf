using System;
using System.Activities;

namespace MyFirstWorkflow
{
    class Program
    {
        static void Main(string[] args)
        {
            Activity workflow = new PortListenerActvity() { Port = 80 };

            Console.WriteLine("Прослушивание порта...");
            try
            {
                WorkflowInvoker.Invoke(workflow);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }
            Console.WriteLine("Прослушивание порта окончено");


            Console.WriteLine("Нажмите клавишу для выхода");
            Console.ReadLine();

            //    AutoResetEvent syncEvent = new AutoResetEvent(false);
            //    AutoResetEvent idleEvent = new AutoResetEvent(false);

            //    var inputs = new Dictionary<string, object>() { { "MaxNumber", 100 } };

            //    WorkflowApplication wfApp =
            //        new WorkflowApplication(new Game(), inputs);

            //    wfApp.Completed = delegate (WorkflowApplicationCompletedEventArgs e)
            //    {
            //        int Turns = Convert.ToInt32(e.Outputs["Turns"]);
            //        Console.WriteLine("Congratulations, you guessed the number in {0} turns.", Turns);

            //        syncEvent.Set();
            //    };

            //    wfApp.Aborted = delegate (WorkflowApplicationAbortedEventArgs e)
            //    {
            //        Console.WriteLine(e.Reason);
            //        syncEvent.Set();
            //    };

            //    wfApp.OnUnhandledException = delegate (WorkflowApplicationUnhandledExceptionEventArgs e)
            //    {
            //        Console.WriteLine(e.UnhandledException.ToString());
            //        return UnhandledExceptionAction.Terminate;
            //    };

            //    wfApp.Idle = delegate (WorkflowApplicationIdleEventArgs e)
            //    {
            //        idleEvent.Set();
            //    };

            //    wfApp.Run();

            //    // Loop until the workflow completes.
            //    WaitHandle[] handles = new WaitHandle[] { syncEvent, idleEvent };
            //    while (WaitHandle.WaitAny(handles) != 0)
            //    {
            //        // Gather the user input and resume the bookmark.
            //        bool validEntry = false;
            //        while (!validEntry)
            //        {
            //            if (!Int32.TryParse(Console.ReadLine(), out int Guess))
            //            {
            //                Console.WriteLine("Please enter an integer.");
            //            }
            //            else
            //            {
            //                validEntry = true;
            //                wfApp.ResumeBookmark("EnterGuess", Guess);
            //            }
            //        }
            //    }
            //    Console.ReadLine();
        }

    }
}
