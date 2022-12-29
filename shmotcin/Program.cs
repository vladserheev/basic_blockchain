using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;

namespace shmotcin
{
    internal static class Program
    {
        public class Block
        {
            public string Data { get; set; }
            public int Index { get; set; }
            private int _nonce;
            public string BlockHash { get; set; }
            public string PreviousBlockHash { get; set; }
            public DateTime Timestamp { get; set; }

            public Block(string data, string previousBlockHash)
            {
                Data = data;
                PreviousBlockHash = previousBlockHash;
                Timestamp = DateTime.Now;
                BlockHash = CreateHash();
                Console.WriteLine("block with Hash: " + BlockHash + " created");
            }

            public void MineBlock(int proofOfWorkDifficulty)
            {
                bool isProofed = false;
                string CurrentHash;
                while (!isProofed)
                {
                    CurrentHash = CreateHash();
                    if (CurrentHash.StartsWith("0000"))
                    {
                        isProofed = true;
                        BlockHash = CurrentHash;
                        Console.WriteLine("block with Hash="+BlockHash+" created");
                    }
                }
            }

            public string CreateHash()
            {

                string rawData = _nonce + Data + PreviousBlockHash + Timestamp;
                    

                string hash = String.Empty;

                // Initialize a SHA256 hash object
                using (SHA256 sha256 = SHA256.Create())
                {
                    // Compute the hash of the given string
                    byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                    // Convert the byte array to string format
                    foreach (byte b in hashValue)
                    {
                        hash += $"{b:X2}";
                    }
                }

                return hash;
            }
        }
        public class Blockchain
        {
            public string Name { get; set; }
            public List<Block> Blocks;

            public Blockchain(string name)
            {
                Name = name;
                Blocks = new List<Block>();
                Blocks.Add(new Block("first Block", "0"));
            }

            public void AddNewBlock(string data)
            {
                Block previousBlock = GetPreviousBlock();
                Blocks.Add(new Block(data, previousBlock.BlockHash));
            }

            public Block GetPreviousBlock()
            {
                return Blocks[Blocks.Count - 1];
            }
        }

        static void Main()
        {
            //Console.OutputEncoding = UTF8Encoding.UTF8;
            Blockchain myBlockchain = new Blockchain("shmotchain");
            myBlockchain.AddNewBlock("chinaga");
        }
    }
}
