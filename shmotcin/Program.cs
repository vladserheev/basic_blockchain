using shmotcin.main;
using shmotcin.DB;
namespace shmotcin
{
    internal static class Program
    {
        static void Main()
        {
            DbAccess.Initialize();
            Blockchain myBlockchain = new Blockchain(4);
           //myBlockchain.AddGenesisBlock();
          //myBlockchain.ClearDB();
            myBlockchain.AddNewTransaction("System", "Vlad", 1000, 0.004);
            myBlockchain.AddNewTransaction("Vlad", "Sasha", 100, 0.004);
            myBlockchain.AddNewTransaction("Vlad", "Masha", 200, 0.004);
            myBlockchain.AddNewTransaction("Masha", "Vlad", 500, 0.004);

            myBlockchain.AddNewBlock();
        
           myBlockchain.GetBalanceByName("Vlad");
            myBlockchain.PrintAllBlocks();
        }
    }
}
