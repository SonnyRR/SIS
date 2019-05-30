namespace IRunes.Data
{
    public class DatabaseConfiguration
    {
        public const string ConnectionStringWindows =
            @"Server=.\SQLEXPRESS;Database=IRunesDB;Trusted_Connection=True;Integrated Security=True;";

        public const string ConnectionStringMacOS =
            @"Server=localhost,1433;Database=IRunesDB;User Id=sa;Password=EmanuelaPreslavaEmilia2";
    }
}
