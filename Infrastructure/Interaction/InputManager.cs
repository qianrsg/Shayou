using System.Collections.Generic;
using System.Threading;

namespace Shayou.Infrastructure.Interaction
{
    public class InputManager
    {
        private Queue<string> inputQueue;
        private int inputFlag;
        private Mutex inputMutex;
        private AutoResetEvent inputReadyEvent;

        public InputManager()
        {
            inputQueue = new Queue<string>();
            inputFlag = 0;
            inputMutex = new Mutex();
            inputReadyEvent = new AutoResetEvent(false);
        }

        public void PostInput(string input)
        {
            inputMutex.WaitOne();
            try
            {
                if (inputFlag == 1)
                {
                    inputQueue.Enqueue(input);
                    inputReadyEvent.Set();
                }
            }
            finally
            {
                inputMutex.ReleaseMutex();
            }
        }

        public void EnableInput()
        {
            inputMutex.WaitOne();
            try
            {
                inputFlag = 1;
            }
            finally
            {
                inputMutex.ReleaseMutex();
            }
        }

        public void DisableInput()
        {
            inputMutex.WaitOne();
            try
            {
                inputFlag = 0;
                inputQueue.Clear();
            }
            finally
            {
                inputMutex.ReleaseMutex();
            }
        }

        public string WaitForInput()
        {
            EnableInput();
            inputReadyEvent.WaitOne();

            inputMutex.WaitOne();
            try
            {
                if (inputQueue.Count > 0)
                {
                    string input = inputQueue.Dequeue();
                    DisableInput();
                    return input;
                }
            }
            finally
            {
                inputMutex.ReleaseMutex();
            }

            DisableInput();
            return null;
        }
    }
}
