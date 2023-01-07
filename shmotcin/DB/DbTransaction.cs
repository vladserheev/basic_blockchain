using System;
using LiteDB;
using shmotcin.main;
namespace shmotcin.DB
{
    internal class DbTransaction
    {
        public static ILiteCollection<Transaction> GetTransactionsPool ()
        {
            var coll = DbAccess.DB.GetCollection<Transaction>(DbAccess.TBL_TRANSACTION_POOL);
            return coll;
        }

        public static void AddTransactionToPool(Transaction transaction)
        {
            var pool = GetTransactionsPool();
            pool.Insert(transaction);
            Console.WriteLine("Transactions added to pool!");
        }

        public static ILiteCollection<Transaction> GetAll()
        {
            var coll = DbAccess.DB.GetCollection<Transaction>(DbAccess.TBL_TRANSACTIONS);
            return coll;
        }

        public static void Add(Transaction transaction)
        {
            var transactions = GetAll();
            transactions.Insert(transaction);
        }
        public static void ReplaceTransactionsFromPoolToList()
        {
            try
            {
                var trxPool = DbAccess.DB.GetCollection<Transaction>(DbAccess.TBL_TRANSACTION_POOL);
                var trxAll = DbAccess.DB.GetCollection<Transaction>(DbAccess.TBL_TRANSACTIONS);

                foreach (var transaction in trxPool.FindAll())
                {
                    trxAll.Insert(transaction);
                }
                Console.WriteLine("All Transactions moved from pool");
            }catch (Exception ex){
                Console.WriteLine("Error: {0}",ex.Message);
            }
        }
        public static void ClearPool()
        {
            var trxPool = GetTransactionsPool();
            trxPool.DeleteAll();
            Console.WriteLine("MemPool cleared!");
        }

    }
}
