namespace Galactic_Colors_Control_Server
{
    public class Client
    {
        public int status = -1;
        public string pseudo = "";
        public int partyID = -1;

        public Party party
        {
            get { if (partyID != -1) { return Server.parties[partyID]; } else { return null; } }
        }
    }
}