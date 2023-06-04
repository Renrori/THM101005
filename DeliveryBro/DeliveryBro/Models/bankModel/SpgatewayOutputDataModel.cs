namespace DeliveryBro.Models.BankModel
{
    public class SpgatewayOutputDataModel
    {

        /// <summary>
        /// 回傳狀態
        /// <para>1.若交易付款成功，則回傳SUCCESS。</para>
        /// <para>2.若交易付款失敗，則回傳錯誤代碼。</para>
        /// <para>3.若使用新增自訂支付欄位之交易，則回傳CUSTOM。</para>
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 回傳訊息(敘述此次交易狀態。)
        /// </summary>
        public string Message { get; set; }

        #region 所有支付方式共同回傳參數

        /// <summary>
        /// 商店代號(智付通商店代號)
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 交易金額
        /// </summary>
        public int Amt { get; set; }

        /// <summary>
        /// 智付通交易序號
        /// <para>智付通在此筆交易取號成功時所產生的序號。</para>
        /// </summary>
        public string TradeNo { get; set; }

        /// <summary>
        /// 商店訂單編號
        /// <para>商店自訂訂單編號。</para>
        /// </summary>
        public string MerchantOrderNo { get; set; }

        /// <summary>
        /// 支付方式
        /// <para>CREDIT 信用卡         即時交易</para>
        /// <para>WEBATM WebATM         即時交易</para>
        /// <para>VACC ATM轉帳        非即時交易</para>
        /// <para>CVS 超商代碼繳費   非即時交易</para>
        /// <para>BARCODE 超商條碼繳費   非即時交易</para>
        /// <para>CVSCOM 超商取貨付款   非即時交易</para>
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// 回傳格式
        /// </summary>
        public string RespondType { get; set; }

        /// <summary>
        /// 支付完成時間
        /// <para>回傳格式為：2014-06-2516:43:49</para>
        /// <para>註 : 若為超商取貨付款的訂單成立時，notify Url不回傳此參數。</para>
        /// </summary>
        public string PayTime { get; set; }

        /// <summary>
        /// 交易 IP
        /// <para>付款人取號或交易時的 IP。</para>
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 款項保管銀行
        /// <para>1.該筆交易履約保證銀行。</para>
        /// <para>2.如商店是直接與銀行簽約的信用卡特約商店，當使用信用卡支付時，本欄位的值會以空值回傳。</para>
        /// <para>3.履保銀行英文代碼與中文名稱對應如下：</para>
        /// <para>  ［Esun］：玉山銀行</para>
        /// <para>  ［Taishin］：台新銀行</para>
        /// <para>  ［HNCB］：華南銀行</para>
        /// </summary>
        public string EscrowBank { get; set; }

        #endregion 所有支付方式共同回傳參數

        #region 信用卡支付回傳參數（包含：Google Pay、Samaung Pay）

        /// <summary>
        /// 金融機構回應碼
        /// <para>1.由收單機構所回應的回應碼。</para>
        /// <para>2.若交易送至收單機構授權時已是失敗狀態，則本欄位的值會以空值回傳。</para>
        /// </summary>
        public string RespondCode { get; set; }

        /// <summary>
        /// 授權碼
        /// <para>1.由收單機構所回應的授權碼。</para>
        /// <para>2.若交易送至收單機構授權時已是失敗狀態，則本欄位的值會以空值回傳。</para>
        /// </summary>
        public string Auth { get; set; }

        /// <summary>
        /// 卡號前六碼
        /// <para>1.信用卡卡號前六碼。</para>
        /// <para>2.若交易送至收單機構授權時已是失敗狀態，則本欄位的值會以空值回傳。</para>
        /// </summary>
        public string Card6No { get; set; }

        /// <summary>
        /// 卡號末四碼
        /// <para>1.信用卡卡號後四碼。</para>
        /// <para>2.若交易送至收單機構授權時已是失敗狀態，則本欄位的值會以空值回傳。</para>
        /// </summary>
        public string Card4No { get; set; }

        /// <summary>
        /// 分期-期別
        /// <para>信用卡分期交易期別。</para>
        /// </summary>
        public int? Inst { get; set; }

        /// <summary>
        /// 分期-首期金額
        /// <para>信用卡分期交易首期金額。</para>
        /// </summary>
        public int? InstFirst { get; set; }

        /// <summary>
        /// 分期-每期金額
        /// <para>信用卡分期交易每期金額。</para>
        /// </summary>
        public int? InstEach { get; set; }

        /// <summary>
        /// ECI 值
        /// <para>1.3D 回傳值 eci=1,2,5,6，代表為 3D 交易。</para>
        /// <para>2.若交易送至收單機構授權時已是失敗狀態，則本欄位的值會以空值回傳。</para>
        /// </summary>
        public string ECI { get; set; }

        /// <summary>
        /// 信用卡快速結帳使用狀態
        /// <para>0=該筆交易為非使用信用卡快速結帳功能</para>
        /// <para>1=該筆交易為首次設定信用卡快速結帳功能</para>
        /// <para>2=該筆交易為使用信用卡快速結帳功能</para>
        /// <para>9=該筆交易為取消信用卡快速結帳功能功能</para>
        /// </summary>
        public int? TokenUseStatus { get; set; }

        /// <summary>
        /// 紅利折抵後實際金額
        /// <para>1.扣除紅利交易折抵後的實際授權金額。</para>
        /// <para>  例：1000 元之交易，紅利折抵 60 元，則紅利折抵後實際金額為 940 元。</para>
        /// <para>2.若紅利點數不足，會有以下狀況：</para>
        /// <para>  2-1 紅利折抵交易失敗，回傳參數數值為 0。</para>
        /// <para>  2-2 紅利折抵交易成功，回傳參數數值為訂單金額。</para>
        /// <para>  2-3 紅利折抵交易是否成功，視該銀行之設定為準。</para>
        /// <para>3.僅有使用紅利折抵交易時才會回傳此參數。</para>
        /// </summary>
        public int? RedAmt { get; set; }

        /// <summary>
        /// 交易類別
        /// <para>將依據此筆交易之信用卡類別回傳相對應的參數，對應參數如下：</para>
        /// <para>   CREDIT = 台灣發卡機構核發之信用卡</para>
        /// <para>   FOREIGN = 國外發卡機構核發之卡</para>
        /// <para>   UNIONPAY = 銀聯卡</para>
        /// <para>   GOOGLEPAY = GooglePay</para>
        /// <para>   SAMSUNGPAY = SamsungPay</para>
        /// <para>   DCC = 動態貨幣轉換</para>
        /// </summary>
        public string PaymentMethod { get; set; }

        /// <summary>
        /// 外幣金額
        /// <para>DCC 動態貨幣轉換交易才會回傳的參數</para>
        /// </summary>
        public double? DCC_Amt { get; set; }

        /// <summary>
        /// 匯率
        /// <para>DCC 動態貨幣轉換交易才會回傳的參數</para>
        /// </summary>
        public double? DCC_Rate { get; set; }

        /// <summary>
        /// 風險匯率
        /// <para>DCC 動態貨幣轉換交易才會回傳的參數</para>
        /// </summary>
        public double? DCC_Markup { get; set; }

        /// <summary>
        /// 幣別
        /// <para>DCC 動態貨幣轉換交易才會回傳的參數</para>
        /// <para>例如：USD、JPY …</para>
        /// </summary>
        public string DCC_Currency { get; set; }

        /// <summary>
        /// 幣別代碼
        /// <para>DCC 動態貨幣轉換交易才會回傳的參數</para>
        /// <para>例如：MOP = 446 ….</para>
        /// </summary>
        public int? DCC_Currency_Code { get; set; }
    }
}
