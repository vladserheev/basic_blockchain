using System;
using System.Security.Cryptography;
using System.Text;

namespace shmotcin.Models
{
    internal class Block
    {
        public int Index { get; set; }
        public string Transactions { get; set; }

        private int _nonce;
        private DateTime _timestamp;
        public string BlockHash { get; private set; }
        public string PreviousBlockHash { get; private set; }

        public Block()
        {

        }
        public Block(int previousBlockIndex, string transactions, string previousBlockHash, int difficulty)
        {
            Index = previousBlockIndex + 1;
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
                // validating data

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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in creating hash: {0}", ex.Message);
                return null;
            }
        }
    }
}
