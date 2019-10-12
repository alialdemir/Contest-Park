namespace ContestPark.Mobile.Models.Balance
{
    public class BalanceCodeModel
    {
        private string _code;

        public string Code
        {
            get { return _code; }
            set { _code = value.ToUpper(); }
        }
    }
}
