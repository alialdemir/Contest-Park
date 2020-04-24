using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Country;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Navigation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Mobile.ViewModels
{
    public class SelectCountryViewModel : ViewModelBase<ObservableGroupCollection<string, CountryModel>>
    {
        #region Constructor

        public SelectCountryViewModel(INavigationService navigationService) : base(navigationService: navigationService)
        {
        }

        #endregion Constructor

        #region Methods

        public override Task InitializeAsync(INavigationParameters parameters = null)
        {
            var items = new List<CountryModel>
            {
                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/AFG_s.png",
                //  Country = "Afghanistan",
                //  PhoneCode = "+93"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ALB_s.png",
                //  Country = "Albania",
                //  PhoneCode = "+355"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/DZA_s.png",
                //  Country = "Algeria",
                //  PhoneCode = "+213"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ASM_s.png",
                //  Country = "American Samoa",
                //  PhoneCode = "+1 684"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/AND_s.png",
                //  Country = "Andorra",
                //  PhoneCode = "+376"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/AGO_s.png",
                //  Country = "Angola",
                //  PhoneCode = "+244"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/AIA_s.png",
                //  Country = "Anguilla",
                //  PhoneCode = "+1 264"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ATA_s.png",
                //  Country = "Antarctica",
                //  PhoneCode = "+672"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ARG_s.png",
                //  Country = "Argentina",
                //  PhoneCode = "+54"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ARM_s.png",
                //  Country = "Armenia",
                //  PhoneCode = "+374"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ABW_s.png",
                //  Country = "Aruba",
                //  PhoneCode = "+297"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/AUS_s.png",
                //  Country = "Australia",
                //  PhoneCode = "+61"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/AUT_s.png",
                //  Country = "Austria",
                //  PhoneCode = "+43"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/AZE_s.png",
                //  Country = "Azerbaijan",
                //  PhoneCode = "+994"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BHS_s.png",
                //  Country = "Bahamas",
                //  PhoneCode = "+1"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BHR_s.png",
                //  Country = "Bahrain",
                //  PhoneCode = "+973"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BGD_s.png",
                //  Country = "Bangladesh",
                //  PhoneCode = "+880"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BRB_s.png",
                //  Country = "Barbados",
                //  PhoneCode = "+1"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BLR_s.png",
                //  Country = "Belarus",
                //  PhoneCode = "+375"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BEL_s.png",
                //  Country = "Belgium",
                //  PhoneCode = "+32"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BLZ_s.png",
                //  Country = "Belize",
                //  PhoneCode = "+501"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BEN_s.png",
                //  Country = "Benin",
                //  PhoneCode = "+229"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BMU_s.png",
                //  Country = "Bermuda",
                //  PhoneCode = "+1"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BTN_s.png",
                //  Country = "Bhutan",
                //  PhoneCode = "+975"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BOL_s.png",
                //  Country = "Bolivia",
                //  PhoneCode = "+591"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BIH_s.png",
                //  Country = "Bosnia and Herzegovina",
                //  PhoneCode = "+387"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BWA_s.png",
                //  Country = "Botswana",
                //  PhoneCode = "+267"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BRA_s.png",
                //  Country = "Brazil",
                //  PhoneCode = "+55"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/VGB_s.png",
                //  Country = "British Virgin Islands",
                //  PhoneCode = "+1 284"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BRN_s.png",
                //  Country = "Brunei",
                //  PhoneCode = "+673"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BGR_s.png",
                //  Country = "Bulgaria",
                //  PhoneCode = "+359"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BFA_s.png",
                //  Country = "Burkina Faso",
                //  PhoneCode = "+226"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BDI_s.png",
                //  Country = "Burundi",
                //  PhoneCode = "+257"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/KHM_s.png",
                //  Country = "Cambodia",
                //  PhoneCode = "+855"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/CMR_s.png",
                //  Country = "Cameroon",
                //  PhoneCode = "+237"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/CAN_s.png",
                //  Country = "Canada",
                //  PhoneCode = "+1"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/CPV_s.png",
                //  Country = "Cape Verde",
                //  PhoneCode = "+238"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/CYM_s.png",
                //  Country = "Cayman Islands",
                //  PhoneCode = "+1-345"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/CAF_s.png",
                //  Country = "Central African Republic",
                //  PhoneCode = "+236"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/CHL_s.png",
                //  Country = "Chile",
                //  PhoneCode = "+56"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/CHN_s.png",
                //  Country = "China",
                //  PhoneCode = "+86"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/COL_s.png",
                //  Country = "Colombia",
                //  PhoneCode = "+57"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/COM_s.png",
                //  Country = "Comoros",
                //  PhoneCode = "+269"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/COK_s.png",
                //  Country = "Cook Islands",
                //  PhoneCode = "+682"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/CRI_s.png",
                //  Country = "Costa Rica",
                //  PhoneCode = "+506"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/HRV_s.png",
                //  Country = "Croatia",
                //  PhoneCode = "+385"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/CUB_s.png",
                //  Country = "Cuba",
                //  PhoneCode = "+53"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/CUW_s.png",
                //  Country = "Curacao",
                //  PhoneCode = "+599"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/CYP_s.png",
                //  Country = "Cyprus",
                //  PhoneCode = "+357"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/CZE_s.png",
                //  Country = "Czech Republic",
                //  PhoneCode = "+420"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/COD_s.png",
                //  Country = "Democratic Republic of Congo",
                //  PhoneCode = "+243"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/DNK_s.png",
                //  Country = "Denmark",
                //  PhoneCode = "+45"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/DJI_s.png",
                //  Country = "Djibouti",
                //  PhoneCode = "+253"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/DMA_s.png",
                //  Country = "Dominica",
                //  PhoneCode = "+1"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/DOM_s.png",
                //  Country = "Dominican Republic",
                //  PhoneCode = "+1"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/TLS_s.png",
                //  Country = "East Timor",
                //  PhoneCode = "+670"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ECU_s.png",
                //  Country = "Ecuador",
                //  PhoneCode = "+593"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/EGY_s.png",
                //  Country = "Egypt",
                //  PhoneCode = "+20"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SLV_s.png",
                //  Country = "El Salvador",
                //  PhoneCode = "+503"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/GNQ_s.png",
                //  Country = "Equatorial Guinea",
                //  PhoneCode = "+240"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ERI_s.png",
                //  Country = "Eritrea",
                //  PhoneCode = "+291"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/EST_s.png",
                //  Country = "Estonia",
                //  PhoneCode = "+372"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ETH_s.png",
                //  Country = "Ethiopia",
                //  PhoneCode = "+251"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/FLK_s.png",
                //  Country = "Falkland Islands",
                //  PhoneCode = "+500"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/FRO_s.png",
                //  Country = "Faroe Islands",
                //  PhoneCode = "+298"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/FJI_s.png",
                //  Country = "Fiji",
                //  PhoneCode = "+679"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/FIN_s.png",
                //  Country = "Finland",
                //  PhoneCode = "+358"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/FRA_s.png",
                //  Country = "France",
                //  PhoneCode = "+33"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/PYF_s.png",
                //  Country = "French Polynesia",
                //  PhoneCode = "+689"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/GAB_s.png",
                //  Country = "Gabon",
                //  PhoneCode = "+241"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/GMB_s.png",
                //  Country = "Gambia",
                //  PhoneCode = "+220"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/GEO_s.png",
                //  Country = "Georgia",
                //  PhoneCode = "+995"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/DEU_s.png",
                //  Country = "Germany",
                //  PhoneCode = "+49"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/GHA_s.png",
                //  Country = "Ghana",
                //  PhoneCode = "+233"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/GIB_s.png",
                //  Country = "Gibraltar",
                //  PhoneCode = "+350"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/GRC_s.png",
                //  Country = "Greece",
                //  PhoneCode = "+30"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/GRL_s.png",
                //  Country = "Greenland",
                //  PhoneCode = "+299"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/GLP_s.png",
                //  Country = "Guadeloupe",
                //  PhoneCode = "+590"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/GUM_s.png",
                //  Country = "Guam",
                //  PhoneCode = "+1 671"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/GTM_s.png",
                //  Country = "Guatemala",
                //  PhoneCode = "+502"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/GIN_s.png",
                //  Country = "Guinea",
                //  PhoneCode = "+224"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/GNB_s.png",
                //  Country = "Guinea-Bissau",
                //  PhoneCode = "+245"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/GUY_s.png",
                //  Country = "Guyana",
                //  PhoneCode = "+592"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/HTI_s.png",
                //  Country = "Haiti",
                //  PhoneCode = "+509"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/HND_s.png",
                //  Country = "Honduras",
                //  PhoneCode = "+504"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/HKG_s.png",
                //  Country = "Hong Kong",
                //  PhoneCode = "+852"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/HUN_s.png",
                //  Country = "Hungary",
                //  PhoneCode = "+36"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ISL_s.png",
                //  Country = "Iceland",
                //  PhoneCode = "+354"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/IND_s.png",
                //  Country = "India",
                //  PhoneCode = "+91"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/IDN_s.png",
                //  Country = "Indonesia",
                //  PhoneCode = "+62"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/IRN_s.png",
                //  Country = "Iran",
                //  PhoneCode = "+98"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/IRQ_s.png",
                //  Country = "Iraq",
                //  PhoneCode = "+964"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/IRL_s.png",
                //  Country = "Ireland",
                //  PhoneCode = "+353"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/IMN_s.png",
                //  Country = "Isle of Man",
                //  PhoneCode = "+44"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ISR_s.png",
                //  Country = "Israel",
                //  PhoneCode = "+972"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ITA_s.png",
                //  Country = "Italy",
                //  PhoneCode = "+39"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/CIV_s.png",
                //  Country = "Ivory Coast",
                //  PhoneCode = "+225"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/JAM_s.png",
                //  Country = "Jamaica",
                //  PhoneCode = "+1"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/JPN_s.png",
                //  Country = "Japan",
                //  PhoneCode = "+81"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/JOR_s.png",
                //  Country = "Jordan",
                //  PhoneCode = "+962"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/KAZ_s.png",
                //  Country = "Kazakhstan",
                //  PhoneCode = "+7"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/KEN_s.png",
                //  Country = "Kenya",
                //  PhoneCode = "+254"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/KIR_s.png",
                //  Country = "Kiribati",
                //  PhoneCode = "+686"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/XKX_s.png",
                //  Country = "Kosovo",
                //  PhoneCode = "+381"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/KWT_s.png",
                //  Country = "Kuwait",
                //  PhoneCode = "+965"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/KGZ_s.png",
                //  Country = "Kyrgyzstan",
                //  PhoneCode = "+996"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/LAO_s.png",
                //  Country = "Laos",
                //  PhoneCode = "+856"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/LVA_s.png",
                //  Country = "Latvia",
                //  PhoneCode = "+371"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/LBN_s.png",
                //  Country = "Lebanon",
                //  PhoneCode = "+961"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/LSO_s.png",
                //  Country = "Lesotho",
                //  PhoneCode = "+266"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/LBR_s.png",
                //  Country = "Liberia",
                //  PhoneCode = "+231"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/LBY_s.png",
                //  Country = "Libya",
                //  PhoneCode = "+218"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/LIE_s.png",
                //  Country = "Liechtenstein",
                //  PhoneCode = "+423"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/LTU_s.png",
                //  Country = "Lithuania",
                //  PhoneCode = "+370"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/LUX_s.png",
                //  Country = "Luxembourg",
                //  PhoneCode = "+352"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MAC_s.png",
                //  Country = "Macau",
                //  PhoneCode = "+853"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MKD_s.png",
                //  Country = "Macedonia",
                //  PhoneCode = "+389"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MDG_s.png",
                //  Country = "Madagascar",
                //  PhoneCode = "+261"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MWI_s.png",
                //  Country = "Malawi",
                //  PhoneCode = "+265"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MYS_s.png",
                //  Country = "Malaysia",
                //  PhoneCode = "+60"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MDV_s.png",
                //  Country = "Maldives",
                //  PhoneCode = "+960"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MLI_s.png",
                //  Country = "Mali",
                //  PhoneCode = "+223"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MLT_s.png",
                //  Country = "Malta",
                //  PhoneCode = "+356"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MHL_s.png",
                //  Country = "Marshall Islands",
                //  PhoneCode = "+692"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MRT_s.png",
                //  Country = "Mauritania",
                //  PhoneCode = "+222"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MUS_s.png",
                //  Country = "Mauritius",
                //  PhoneCode = "+230"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MEX_s.png",
                //  Country = "Mexico",
                //  PhoneCode = "+52"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/FSM_s.png",
                //  Country = "Micronesia",
                //  PhoneCode = "+691"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MDA_s.png",
                //  Country = "Moldova",
                //  PhoneCode = "+373"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MCO_s.png",
                //  Country = "Monaco",
                //  PhoneCode = "+377"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MNG_s.png",
                //  Country = "Mongolia",
                //  PhoneCode = "+976"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MNE_s.png",
                //  Country = "Montenegro",
                //  PhoneCode = "+382"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MSR_s.png",
                //  Country = "Montserrat",
                //  PhoneCode = "+1 664"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MAR_s.png",
                //  Country = "Morocco",
                //  PhoneCode = "+212"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MOZ_s.png",
                //  Country = "Mozambique",
                //  PhoneCode = "+258"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MMR_s.png",
                //  Country = "Myanmar [Burma]",
                //  PhoneCode = "+95"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/NAM_s.png",
                //  Country = "Namibia",
                //  PhoneCode = "+264"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/NRU_s.png",
                //  Country = "Nauru",
                //  PhoneCode = "+674"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/NPL_s.png",
                //  Country = "Nepal",
                //  PhoneCode = "+977"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/NLD_s.png",
                //  Country = "Netherlands",
                //  PhoneCode = "+31"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/NCL_s.png",
                //  Country = "New Caledonia",
                //  PhoneCode = "+687"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/NZL_s.png",
                //  Country = "New Zealand",
                //  PhoneCode = "+64"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/NIC_s.png",
                //  Country = "Nicaragua",
                //  PhoneCode = "+505"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/NER_s.png",
                //  Country = "Niger",
                //  PhoneCode = "+227"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/NGA_s.png",
                //  Country = "Nigeria",
                //  PhoneCode = "+234"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/NIU_s.png",
                //  Country = "Niue",
                //  PhoneCode = "+683"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/NFK_s.png",
                //  Country = "Norfolk Island",
                //  PhoneCode = "+672"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/PRK_s.png",
                //  Country = "North Korea",
                //  PhoneCode = "+850"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MNP_s.png",
                //  Country = "Northern Mariana Islands",
                //  PhoneCode = "+1 670"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/NOR_s.png",
                //  Country = "Norway",
                //  PhoneCode = "+47"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/OMN_s.png",
                //  Country = "Oman",
                //  PhoneCode = "+968"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/PAK_s.png",
                //  Country = "Pakistan",
                //  PhoneCode = "+92"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/PLW_s.png",
                //  Country = "Palau",
                //  PhoneCode = "+680"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/PAN_s.png",
                //  Country = "Panama",
                //  PhoneCode = "+507"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/PNG_s.png",
                //  Country = "Papua New Guinea",
                //  PhoneCode = "+675"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/PRY_s.png",
                //  Country = "Paraguay",
                //  PhoneCode = "+595"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/PER_s.png",
                //  Country = "Peru",
                //  PhoneCode = "+51"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/PHL_s.png",
                //  Country = "Philippines",
                //  PhoneCode = "+63"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/PCN_s.png",
                //  Country = "Pitcairn Islands",
                //  PhoneCode = "+870"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/POL_s.png",
                //  Country = "Poland",
                //  PhoneCode = "+48"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/PRT_s.png",
                //  Country = "Portugal",
                //  PhoneCode = "+351"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/PRI_s.png",
                //  Country = "Puerto Rico",
                //  PhoneCode = "+1"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/QAT_s.png",
                //  Country = "Qatar",
                //  PhoneCode = "+974"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/COG_s.png",
                //  Country = "Republic of the Congo",
                //  PhoneCode = "+242"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/REU_s.png",
                //  Country = "Reunion",
                //  PhoneCode = "+262"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ROU_s.png",
                //  Country = "Romania",
                //  PhoneCode = "+40"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/RUS_s.png",
                //  Country = "Russia",
                //  PhoneCode = "+7"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/RWA_s.png",
                //  Country = "Rwanda",
                //  PhoneCode = "+250"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/BLM_s.png",
                //  Country = "Saint Barthélemy",
                //  PhoneCode = "+590"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SHN_s.png",
                //  Country = "Saint Helena",
                //  PhoneCode = "+290"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/KNA_s.png",
                //  Country = "Saint Kitts and Nevis",
                //  PhoneCode = "+1"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/LCA_s.png",
                //  Country = "Saint Lucia",
                //  PhoneCode = "+1"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/MAF_s.png",
                //  Country = "Saint Martin",
                //  PhoneCode = "+1 599"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SPM_s.png",
                //  Country = "Saint Pierre and Miquelon",
                //  PhoneCode = "+508"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/VCT_s.png",
                //  Country = "Saint Vincent and the Grenadines",
                //  PhoneCode = "+1"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/WSM_s.png",
                //  Country = "Samoa",
                //  PhoneCode = "+685"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SMR_s.png",
                //  Country = "San Marino",
                //  PhoneCode = "+378"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/STP_s.png",
                //  Country = "Sao Tome and Principe",
                //  PhoneCode = "+239"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SAU_s.png",
                //  Country = "Saudi Arabia",
                //  PhoneCode = "+966"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SEN_s.png",
                //  Country = "Senegal",
                //  PhoneCode = "+221"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SRB_s.png",
                //  Country = "Serbia",
                //  PhoneCode = "+381"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SYC_s.png",
                //  Country = "Seychelles",
                //  PhoneCode = "+248"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SLE_s.png",
                //  Country = "Sierra Leone",
                //  PhoneCode = "+232"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SGP_s.png",
                //  Country = "Singapore",
                //  PhoneCode = "+65"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SVK_s.png",
                //  Country = "Slovakia",
                //  PhoneCode = "+421"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SVN_s.png",
                //  Country = "Slovenia",
                //  PhoneCode = "+386"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SLB_s.png",
                //  Country = "Solomon Islands",
                //  PhoneCode = "+677"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SOM_s.png",
                //  Country = "Somalia",
                //  PhoneCode = "+252"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ZAF_s.png",
                //  Country = "South Africa",
                //  PhoneCode = "+27"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/KOR_s.png",
                //  Country = "South Korea",
                //  PhoneCode = "+82"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SSD_s.png",
                //  Country = "South Sudan",
                //  PhoneCode = "+211"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ESP_s.png",
                //  Country = "Spain",
                //  PhoneCode = "+34"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/LKA_s.png",
                //  Country = "Sri Lanka",
                //  PhoneCode = "+94"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SDN_s.png",
                //  Country = "Sudan",
                //  PhoneCode = "+249"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SUR_s.png",
                //  Country = "Suriname",
                //  PhoneCode = "+597"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SWZ_s.png",
                //  Country = "Swaziland",
                //  PhoneCode = "+268"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SWE_s.png",
                //  Country = "Sweden",
                //  PhoneCode = "+46"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/CHE_s.png",
                //  Country = "Switzerland",
                //  PhoneCode = "+41"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/SYR_s.png",
                //  Country = "Syria",
                //  PhoneCode = "+963"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/TWN_s.png",
                //  Country = "Taiwan",
                //  PhoneCode = "+886"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/TJK_s.png",
                //  Country = "Tajikistan",
                //  PhoneCode = "+992"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/TZA_s.png",
                //  Country = "Tanzania",
                //  PhoneCode = "+255"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/THA_s.png",
                //  Country = "Thailand",
                //  PhoneCode = "+66"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/TGO_s.png",
                //  Country = "Togo",
                //  PhoneCode = "+228"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/TKL_s.png",
                //  Country = "Tokelau",
                //  PhoneCode = "+690"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/TTO_s.png",
                //  Country = "Trinidad and Tobago",
                //  PhoneCode = "+1"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/TUN_s.png",
                //  Country = "Tunisia",
                //  PhoneCode = "+216"
                //},

                new CountryModel
                {
                  Flag = "assets/images/TUR_s.png",
                  Country = "Turkey",
                  PhoneCode = "+90"
                },

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/TKM_s.png",
                //  Country = "Turkmenistan",
                //  PhoneCode = "+993"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/TUV_s.png",
                //  Country = "Tuvalu",
                //  PhoneCode = "+688"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/UGA_s.png",
                //  Country = "Uganda",
                //  PhoneCode = "+256"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/UKR_s.png",
                //  Country = "Ukraine",
                //  PhoneCode = "+380"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ARE_s.png",
                //  Country = "United Arab Emirates",
                //  PhoneCode = "+971"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/GBR_s.png",
                //  Country = "United Kingdom",
                //  PhoneCode = "+44"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/USA_s.png",
                //  Country = "United States",
                //  PhoneCode = "+1"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/URY_s.png",
                //  Country = "Uruguay",
                //  PhoneCode = "+598"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/UZB_s.png",
                //  Country = "Uzbekistan",
                //  PhoneCode = "+998"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/VUT_s.png",
                //  Country = "Vanuatu",
                //  PhoneCode = "+678"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/VAT_s.png",
                //  Country = "Vatican",
                //  PhoneCode = "+39"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/VEN_s.png",
                //  Country = "Venezuela",
                //  PhoneCode = "+58"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/VNM_s.png",
                //  Country = "Vietnam",
                //  PhoneCode = "+84"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ESH_s.png",
                //  Country = "Western Sahara",
                //  PhoneCode = "+212"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/YEM_s.png",
                //  Country = "Yemen",
                //  PhoneCode = "+967"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ZMB_s.png",
                //  Country = "Zambia",
                //  PhoneCode = "+260"
                //},

                //new CountryModel
                //{
                //  Flag = "http://www.countryareacode.net/pictures/flag/ZWE_s.png",
                //  Country = "Zimbabwe",
                //  PhoneCode = "+263"
                //},
            };

            ServiceModel = new Models.ServiceModel.ServiceModel<ObservableGroupCollection<string, CountryModel>>
            {
                Items = items.OrderBy(p => p.Country)
                   .GroupBy(p => p.Country[0].ToString())
                   .Select(p => new ObservableGroupCollection<string, CountryModel>(p))
                   .ToList()
            };

            return base.InitializeAsync(parameters);
        }

        #endregion Methods
    }
}
