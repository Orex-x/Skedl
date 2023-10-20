namespace Skedl.App.Services
{
    public class EventProvider
    {
        public event EventHandler UserDataReady;
        public event EventHandler UserLogout;

        public EventProvider()
        {

        }

        public void CallUserDataReady()
        {
            UserDataReady.Invoke(this, new EventArgs());
        }

        public void CallUserLogout()
        {
            UserLogout.Invoke(this, new EventArgs());
        }
    }
}
