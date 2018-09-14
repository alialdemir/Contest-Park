using Orleans.TestKit;

namespace ContestPark.Infrastructure.Category.Tests
{
    public class TestGrainBase : TestKitBase
    {
        public string WitcherUserId
        {
            get { return "1111-1111-1111-1111"; }
        }

        public string BotUserId
        {
            get { return "3333-3333-3333-bot"; }
        }

        public string DemoUserId
        {
            get { return "2222-2222-2222-2222"; }
        }

        // Should_BeklenenDavranış_When_Koşul
        //  When_Koşul_Expect_Beklenen davranı
        // Given_ÖnHazırlık_When_Koşul_Then_BeklenenDavranış
        // MethodAdı_Senaryo_Sonuc
        // MethodAdı_sonuç_senarya

        ///// <summary>
        /////
        ///// </summary>
        //[TestMethod]
        //public void x()
        //{
        //// Arrange

        //// Act

        //// Assert
        //}
    }
}