namespace Skedl.App.Services
{
    public class EventProvider
    {
        public event EventHandler UserDataReady;
        public event EventHandler UserLogout;

        public event EventHandler LoadGroups;

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
        
        public void CallLoadGroups()
        {
            LoadGroups.Invoke(this, new EventArgs());
        }
    }
}
