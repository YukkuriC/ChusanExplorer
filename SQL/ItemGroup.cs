using System;
using System.Collections.Generic;

namespace ChusanExplorer
{
    public class ItemGroup : Dictionary<int, int>
    {
        public int itemType, userId;

        public string SQLFetch => $"select * from chusan_user_item where user_id={userId} and item_kind={itemType}";
        public string SQLCreate(int newId, int initCount = 1) => $"insert into chusan_user_item (user_id,item_kind,item_id,is_valid,stock) values ({userId},{itemType},{newId},1,{initCount})";
        public string SQLRemove(int delId) => $"delete from chusan_user_item where user_id={userId} and item_kind={itemType} and item_id={delId}";
        public string SQLAlter(int itemId, int newCount) => $"update chusan_user_item set stock={newCount} where user_id={userId} and item_kind={itemType} and item_id={itemId}";

        public ItemGroup(int user, int type) : base()
        {
            userId = user;
            itemType = type;
            Init();
        }

        public void Init()
        {
            Clear();
            using (var r = DBLoader.Read(SQLFetch))
                while (r.Read())
                    this[Convert.ToInt32(r["item_id"])] = Convert.ToInt32(r["stock"]);
        }
    }
}
