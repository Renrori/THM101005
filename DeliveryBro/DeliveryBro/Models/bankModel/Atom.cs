namespace DeliveryBro.Models.bankModel
{
    public class Atom<T>
    {
        /// <summary>執行是否成功</summary>
        public bool IsSuccess { get; set; }

        /// <summary>是否為錯誤狀態</summary>
        public bool IsError { get; set; }

        /// <summary>訊息</summary>
        public string Message { get; set; }

        /// <summary>項目</summary>
        public T Item { get; set; }
    }
}

