using System;
using LiteDB;
using shmotcin.main;

namespace shmotcin.DB
{
    internal class DbBlocks
    {
        public static ILiteCollection<Block> GetBlocks()
        {
            try
            {
                var coll = DbAccess.DB.GetCollection<Block>(DbAccess.TBL_BLOCKS);
                coll.EnsureIndex(x => x.Index);
                return coll;
            }catch(Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                return null;
            }
        }

        public static void AddBlock(Block block)
        {
            try
            {
                var coll = DbAccess.DB.GetCollection<Block>(DbAccess.TBL_BLOCKS);
                coll.Insert(block);
            }catch(Exception ex)
            {
                Console.WriteLine("Error: {0}",ex.Message);
            }
        }

        public static Block GetLastBlock()
        {
            var blocks = GetBlocks();
            var block = blocks.FindOne(Query.All(Query.Descending));

            return block;
        }
    }
}
