using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;

namespace shmotcin
{
    internal static class Program
    {
        public class Transaction
        {
            public string Sender { get;private set; }
            public string Receiver { get; private set; }
            public double Amount { get; private set; }
            public double Fee { get; private set; }

            public Transaction(string sender, string receiver, double amount, double fee)
            {
                Sender = sender;
                Receiver = receiver;
                Amount = amount;
                Fee = fee;
            }
        }
        public class Block
        {
            public int Index { get; set; }
            public Transaction[] Transactions { get; set; }
            
            private int _nonce;
            private DateTime _timestamp;
            public string BlockHash { get; private set; }
            public string PreviousBlockHash { get; private set; }
            

            public Block(int index, Transaction[] transactions, string previousBlockHash, int difficulty)
            {
                Index = index;
                Transactions = transactions;
                PreviousBlockHash = previousBlockHash;
                _timestamp = DateTime.Now;

                string responseFromMining = MineBlock(difficulty);
                if (responseFromMining == null)
                    Console.WriteLine("Failed!. Block hasn't been created");
                else
                    BlockHash = responseFromMining;
                    Console.WriteLine("block with Hash: " + BlockHash + " created");
            }

            private string MineBlock(int difficulty)
            {
                string blockHash = "";
                string currentHash;

                // Keep mining until a valid hash is found
                do
                {
                    currentHash = CreateHash();
                    if (currentHash == null)
                        return null; ;
                    _nonce++;
                } while (!currentHash.StartsWith(new string('0', difficulty)));

                blockHash = currentHash;
                return blockHash;
            }

            private string CreateHash()
            {
                try
                {
                    string rawData = _nonce + PreviousBlockHash + _timestamp;
                    string hash = String.Empty;

                    using (SHA256 sha256 = SHA256.Create())
                    {
                        byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                        foreach (byte b in hashValue)
                        {
                            hash += $"{b:X2}";
                        }
                    }
                    return hash;
                }catch (Exception ex)
                {
                    Console.WriteLine("Error in creating hash: {0}", ex.Message);
                    return null;
                }
            }
        }

        public class Blockchain
        {
            public List<Block> Blocks;
            public List<Transaction> Transactions;
            private int _difficulty;
            public Blockchain(int difficulty)
            {
                Blocks = new List<Block>();
                Transactions = new List<Transaction>();
                _difficulty = difficulty;
            }

            public void AddGenesisBlock()
            {
                AddNewTransaction("System", "Genesis Account", 1000, 0.001);
                Blocks.Add(new Block(Blocks.Count, Transactions.ToArray(), "0", _difficulty));
            }
            public void AddNewBlock(Transaction[] transactions)
            {
                Block previousBlock = GetPreviousBlock();
                Blocks.Add(new Block(Blocks.Count, transactions, previousBlock.BlockHash, _difficulty));
                Transactions.Clear();
            }

            public void AddNewTransaction(string sender, string reciever, double amount, double fee)
            {
                Transaction transaction = new Transaction(sender, reciever, amount, fee);
                Transactions.Add(transaction);
            }

            public void IsChainValid()
            {
                int blockIndex = 1;
                while (blockIndex < Blocks.Count)
                {
                    if(Blocks[blockIndex].PreviousBlockHash != Blocks[blockIndex - 1].BlockHash)
                    {
                        Console.WriteLine("Error! invalid blockchain");
                        return;
                    }
                    blockIndex++;
                }
            }

            public double GetBalanceByName(string name)
            {
                double spent = 0;
                double income = 0;
                double balance = 0;
                double totalBalance = 0;
                foreach(var block in Blocks)
                {
                    var transactions = block.Transactions;
                    foreach(var transaction in transactions)
                    {
                        if (String.Equals(name.ToLower(), transaction.Sender.ToLower()))
                        {
                            spent+=transaction.Amount;
                        }
                        if (String.Equals(name.ToLower(), transaction.Receiver.ToLower()))
                        {
                            income += transaction.Amount;
                        }
                        balance = income - spent;
                    }
                    totalBalance = totalBalance + balance;
                }
                Console.WriteLine("Balance: {0}", totalBalance);
                return totalBalance;
            }

            public void PrintAllBlocks()
            {
                foreach(Block block in Blocks)
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

            private Block GetPreviousBlock()
            {
                return Blocks[Blocks.Count - 1];
            }
        }

        static void Main()
        {
            Blockchain myBlockchain = new Blockchain(4);
            myBlockchain.AddGenesisBlock();

            myBlockchain.AddNewTransaction("System", "Vlad", 1000, 0.004);
            myBlockchain.AddNewTransaction("Vlad", "Sasha", 100, 0.004);
            myBlockchain.AddNewTransaction("Vlad", "Masha", 200, 0.004);
            myBlockchain.AddNewTransaction("Masha", "Vlad", 500, 0.004);

            myBlockchain.AddNewBlock(myBlockchain.Transactions.ToArray());

            myBlockchain.GetBalanceByName("Vlad");
            myBlockchain.PrintAllBlocks();
        }
    }
}
