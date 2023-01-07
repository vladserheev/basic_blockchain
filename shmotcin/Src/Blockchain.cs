using System;
using System.Collections.Generic;
using LiteDB;
using shmotcin.DB;
using Newtonsoft.Json;

namespace shmotcin.main
{
    internal class Blockchain
    {
        private int _difficulty;

        public Blockchain(int difficulty)
        {
            _difficulty = difficulty;

            Initialize();
        }

        private void Initialize()
        {
            var blocks = GetBlocks();
            if (blocks == null)
            {
                return;
            }
            // If blockchain has no items => create first(genesis) Block
            if (blocks.Count() < 1)
            {
                Console.WriteLine("blocks count < 1");
                AddGenesisBlock();
            }
        }

        public void AddGenesisBlock()
        {
            try
            {
                CreateGenesisTransactions();

                var trxPool = DbTransaction.GetTransactionsPool();
                string tempTransactions = JsonConvert.SerializeObject(trxPool.FindAll());

                Block block = new Block(0, tempTransactions, "0", _difficulty);
                DbBlocks.AddBlock(block);
                DbTransaction.ReplaceTransactionsFromPoolToList();
                DbTransaction.ClearPool();

                Console.WriteLine("Genesis block successfully created to DB!");
            }catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void CreateGenesisTransactions()
        {
            Transaction tr1 = new Transaction()
            {
                Sender = "System",
                Receiver = "user1",
                Amount = 1000,
                Fee = 0
            };
            DbTransaction.AddTransactionToPool(tr1);
            Transaction tr2 = new Transaction()
            {
                Sender = "System",
                Receiver = "user2",
                Amount = 2000,
                Fee = 0
            };
            DbTransaction.AddTransactionToPool(tr2);
        }

        public void AddNewBlock()
        {
            Block previousBlock = DbBlocks.GetLastBlock();

            var trxPool = DbTransaction.GetTransactionsPool();
            string tempTransactions = JsonConvert.SerializeObject(trxPool.FindAll());

            DbBlocks.AddBlock(new Block(previousBlock.Index, tempTransactions, previousBlock.BlockHash, _difficulty));
            Console.WriteLine("Block succeessfully added!");

            DbTransaction.ReplaceTransactionsFromPoolToList();
            DbTransaction.ClearPool();

        }

        public void AddNewTransaction(string sender, string reciever, double amount, double fee)
        {

            Transaction transaction = new Transaction()
            {
                Sender = sender,
                Receiver = reciever,
                Amount = amount,
                Fee = fee
            };
            DbTransaction.AddTransactionToPool(transaction);
        }

        public bool IsChainValid()
        {
            var blocks = GetBlocks().FindAll();
            string previousHash = "0";
            foreach(Block block in blocks)
            {
                if(block.PreviousBlockHash != previousHash)
                {
                    return false;
                }
                previousHash = block.PreviousBlockHash;
            }
            return true;
        }

        public double GetBalanceByName(string name)
        {
            var blocks = GetBlocks();
            double spent = 0;
            double income = 0;
            double balance = 0;
            foreach (var block in blocks.FindAll())
            {
                var transactions = JsonConvert.DeserializeObject<Transaction[]>(block.Transactions);
                foreach (var transaction in transactions)
                {
                    if (String.Equals(name.ToLower(), transaction.Sender.ToLower()))
                    {
                        spent += transaction.Amount;
                    }
                    if (String.Equals(name.ToLower(), transaction.Receiver.ToLower()))
                    {
                        income += transaction.Amount;
                    }
                    balance = income - spent;
                }
            }
            Console.WriteLine("{0} balance: {1}",name, balance);
            return balance;
        }

        public void PrintAllBlocks()
        {
            var blocks = GetBlocks();
            Console.WriteLine("======= All Blocks =======");
            foreach (Block block in blocks.FindAll())
            {
                PrintBlock(block);
            }
        }

        private void PrintBlock(Block block)
        {
            Console.WriteLine("Index: {0}", block.Index);
            Console.WriteLine("Hash: {0}", block.BlockHash);
            Console.WriteLine("PrevHash: {0}", block.PreviousBlockHash);
            Console.WriteLine("Transactions: {0}", block.Transactions.ToString());
        }

        public static ILiteCollection<Block> GetBlocks()
        {
            return DbBlocks.GetBlocks();
        }

        public void ClearDB()
        {
            DbAccess.ClearDB();
            Console.WriteLine("Db cleared!");
        }
    }
}
