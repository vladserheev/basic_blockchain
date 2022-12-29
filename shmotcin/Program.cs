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
                BlockHash = MineBlock();
                Console.WriteLine("block with Hash: " + BlockHash + " created");
            }

            public string MineBlock()
            {
                bool isProofed = false;
                string blockHash="";
                string CurrentHash;
                while (!isProofed)
                {
                    CurrentHash = CreateHash();
                    if (CurrentHash.StartsWith("0000"))
                    {
                        isProofed = true;
                        blockHash = CurrentHash;
                    }
                    _nonce++;
                }
                return blockHash;
            }

            public string CreateHash()
            {
                string rawData = _nonce + Data + PreviousBlockHash + Timestamp;
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

            public Block GetPreviousBlock()
            {
                return Blocks[Blocks.Count - 1];
            }
        }

        static void Main()
        {
            Blockchain myBlockchain = new Blockchain("mychain");
            myBlockchain.AddNewBlock("block1");
            myBlockchain.AddNewBlock("block2");
            myBlockchain.AddNewBlock("block3");
        }
    }
}
