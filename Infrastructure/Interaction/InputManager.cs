using System.Collections.Generic;
using System.Threading;
using Shayou.Infrastructure.Interaction.Contracts;

namespace Shayou.Infrastructure.Interaction
{
    public class InputManager
    {
        private Queue<string> inputQueue;
        private int inputFlag;
        private Mutex inputMutex;
        private AutoResetEvent inputReadyEvent;
        public InputRequestPacket? CurrentRequest { get; private set; }

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

        public string WaitForInput(InputRequestPacket requestPacket)
        {
            CurrentRequest = requestPacket;

            inputMutex.WaitOne();
            try
            {
                inputFlag = 1;

                if (inputQueue.Count > 0)
                {
                    string input = inputQueue.Dequeue();
                    inputFlag = 0;
                    CurrentRequest = null;
                    return input;
                }
            }
            finally
            {
                inputMutex.ReleaseMutex();
            }

            inputReadyEvent.WaitOne();

            inputMutex.WaitOne();
            try
            {
                inputFlag = 0;

                if (inputQueue.Count > 0)
                {
                    string input = inputQueue.Dequeue();
                    CurrentRequest = null;
                    return input;
                }
                else
                {
                    CurrentRequest = null;
                    return null;
                }
            }
            finally
            {
                inputMutex.ReleaseMutex();
            }
        }
    }
}
