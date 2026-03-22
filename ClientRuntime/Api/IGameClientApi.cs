namespace Shayou.ClientRuntime.Api
{
    public interface IGameClientApi
    {
        void SendCommand(string key);

        void CreateRoom();

        void JoinRoom();

        void Ready();

        void CancelReady();

        void StartGame();

        void Pass();

        void Confirm();

        void Cancel();
    }
}
