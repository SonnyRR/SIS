namespace SULS.Data
{
    public class DatabaseConfiguration
    {
        public const string ConnectionString =
            @"Server=.\SQLEXPRESS;Database=SulsDB-VasilKotsev;Trusted_Connection=True;Integrated Security=True;";

        public const string ConnectionStringMacOS =
            @"Server=localhost,1433;Database=SulsDB-VasilKotsev;User Id=sa;Password=EmanuelaPreslavaEmilia2;";

    }
}
