using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Globalization;


//T--------------------------------------------------------------------------------------------tłumaczenie zmiennych z google translatora ;]
//  narastający przychód                    - growing Revenue           - txtGrowRevenue  
//  narastający koszt                       - increasing cost           - txtInCost
//  miesięczny przychód                     - monthly Revenue           - txtMonthRevenue  
//  miesięczny dochód                       - monthly Income            - Revenue
//  miesięczny koszt                        - monthly cost              - txtMonthCost
//  całkowity przychód                      - totoal Revenue            - txtTotalIcome
//  całkowity koszt                         - totoal cost               - txtTotalCost
//  ubezpieczenie zdrowotne                 - health insurance          - txtHIns
//  ubezpieczenie społeczne                 - social insurance          - txtSIns
//  ZdrOdl - kwota podlegająca odliczeniu   - amount deductible         - txtAmountDed   amD
//  rata/zaliczka podatku                   - tax prepayment            - txtTaxPrepay
//T--------------------------------------------------------------------------------------------tłumaczenie zmiennych z google translatora ;]

namespace podatekLiniowySQL
{
    public partial class plSql : Form
    {
        private const decimal taxRate = 0.19m;      // stawka podatku
        private bool mouseDown;                     // zmienna do przesówania okna
        private Point lastLocation;                 // -------------||----------

        //zmienne do bazy danych
        private SQLiteConnection dbConnect;  // obiekt bazy
        private SQLiteCommand dbCommand;     // wysyłanie zapytań do bazy
        private SQLiteDataReader dbReader;   // przechowuje informacje zwracane dla zapytania
        private string dbQuerry = null;      // komendy - zapytania do bazy

        public plSql()
        {
            InitializeComponent();
        }

        private void plSql_Load(object sender, EventArgs e)
        {
            LoadAllFromDB();
            timer2.Start();
            ZeroIfEmpty();
        }

        // ------------------------------------------------------------------------ sprawdzanie - tworzenie bazy danych ---------------------

        //połączenie z bazą danych
        private void ConectionStrOpen()
        {
            dbConnect = new SQLiteConnection("Data Source=taxDB.db;Version=3;New=False;Compress=True;");
            dbConnect.Open();
        }

        //sprawdza czy istniej plik bazy
        private void CheckDb()
        {
            try
            {
                if (File.Exists("taxDB.db"))                                    //sprawdzamy czy istnieje plik bazy danych
                {
                    timer1.Start();
                    lblInfo.Text = "... Plik bazy danych już istnieje ...";
                }
                else
                {
                    CreateDB();
                    InsertZeroDB();
                }
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.ToString();
            }
        }

        //tworzy baze i tabele
        private void CreateDB()
        {
            SQLiteConnection.CreateFile("taxDB.db");                    //jeżeli pliku nie ma - to go tworzymy

            ConectionStrOpen();

            dbQuerry = "CREATE TABLE 'tax2019' ('Id' INTEGER PRIMARY KEY AUTOINCREMENT, 'Month' INTEGER, 'Revenue' NUMERIC, 'Cost' NUMERIC, 'Social' NUMERIC, 'Health' NUMERIC, 'Paid' NUMERIC)"; //tworzenie tabeli
            dbCommand = new SQLiteCommand(dbQuerry, dbConnect);         // wysłanie polecenia
            dbCommand.ExecuteNonQuery();                                // wykona nie polecenia
            dbConnect.Close();

            timer1.Start();
            lblInfo.Text = "... Utworzono bazę danych i tabelę ...";
        }

        //wstawia zera do tabeli, zeby nie krzyczało po wczytaniu
        private void InsertZeroDB()
        {
            ConectionStrOpen();

            for (int i = 1; i < 13; i++)
            {
                dbQuerry = " INSERT INTO 'tax2019' (Month, Revenue, Cost, Social, Health) VALUES ( '" + i + "', 0.00, 0.00, 0.00, 0.00, 0.00)";    // wprowadza 1-12 do miesiecy i  zera do reszty pól tabeli
                dbCommand = new SQLiteCommand(dbQuerry, dbConnect);                                                              // wysłanie polecenia
                dbCommand.ExecuteNonQuery();                                                                                     // wykona nie polecenia                         
            }
            dbConnect.Close();
        }


        private void SaveRevDB()
        {
            try
            {
                ConectionStrOpen();

                int i = 1;
                //tworzy liste z textboxami kosztów
                List<TextBox> txtGrowRevenueList = new List<TextBox>(12)
                {
                    txtGrowRevenue1,
                    txtGrowRevenue2,
                    txtGrowRevenue3,
                    txtGrowRevenue4,
                    txtGrowRevenue5,
                    txtGrowRevenue6,
                    txtGrowRevenue7,
                    txtGrowRevenue8,
                    txtGrowRevenue9,
                    txtGrowRevenue10,
                    txtGrowRevenue11,
                    txtGrowRevenue12
                };
                //dodaje przychody do tabeli
                foreach (TextBox item in txtGrowRevenueList)
                {
                    dbQuerry = "Update 'tax2019' SET Revenue=@itemtext where Month=@i ";
                    dbCommand = new SQLiteCommand(dbQuerry, dbConnect);         // wysłanie polecenia
                    dbCommand.Parameters.AddWithValue("@itemtext", item.Text.Replace(",", "."));
                    dbCommand.Parameters.AddWithValue("@i", i);
                    dbCommand.ExecuteNonQuery();                                // wykonanie polecenia
                    i++;
                }
                dbConnect.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //zapisywanie do tabeli kosztów
        private void SaveCostDB()
        {
            try
            {
                ConectionStrOpen();

                int i = 1;
                //tworzy liste z textboxami kosztów
                List<TextBox> txtInCostList = new List<TextBox>(12)
                {
                    txtInCost1,
                    txtInCost2,
                    txtInCost3,
                    txtInCost4,
                    txtInCost5,
                    txtInCost6,
                    txtInCost7,
                    txtInCost8,
                    txtInCost9,
                    txtInCost10,
                    txtInCost11,
                    txtInCost12
                };
                //dodaje przychody do tabeli
                foreach (TextBox item in txtInCostList)
                {
                    dbQuerry = "Update 'tax2019' SET Cost=@itemtext where Month=@i";
                    dbCommand = new SQLiteCommand(dbQuerry, dbConnect);         // wysłanie polecenia
                    dbCommand.Parameters.AddWithValue("@itemtext", item.Text.Replace(",", "."));
                    dbCommand.Parameters.AddWithValue("@i", i);
                    dbCommand.ExecuteNonQuery();                                // wykonanie polecenia
                    i++;
                }
                dbConnect.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //zapisywanie do tabeli ZUS - społ.
        private void SaveSocial()
        {
            try
            {
                ConectionStrOpen();

                int i = 1;
                //tworzy liste z textboxami ZUS -społ
                List<TextBox> txtSInsList = new List<TextBox>(12)
                {
                    txtSIns1,
                    txtSIns2,
                    txtSIns3,
                    txtSIns4,
                    txtSIns5,
                    txtSIns6,
                    txtSIns7,
                    txtSIns8,
                    txtSIns9,
                    txtSIns10,
                    txtSIns11,
                    txtSIns12
                };
                //dodaje zus - społ do tabeli
                foreach (TextBox item in txtSInsList)
                {
                    dbQuerry = "Update 'tax2019' SET Social=@itemtext where Month=@i";
                    dbCommand = new SQLiteCommand(dbQuerry, dbConnect);         // wysłanie polecenia
                    dbCommand.Parameters.AddWithValue("@itemtext", item.Text.Replace(",", "."));
                    dbCommand.Parameters.AddWithValue("@i", i);
                    dbCommand.ExecuteNonQuery();                                // wykonanie polecenia
                    i++;
                }
                dbConnect.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //zapisywanie do tabeli ZUS- zdr.
        private void SaveHealth()
        {
            try
            {
                ConectionStrOpen();

                int i = 1;
                //tworzy liste z textboxami ZUS -zdro
                List<TextBox> txtHInsList = new List<TextBox>(12)
                {
                    txtHIns1,
                    txtHIns2,
                    txtHIns3,
                    txtHIns4,
                    txtHIns5,
                    txtHIns6,
                    txtHIns7,
                    txtHIns8,
                    txtHIns9,
                    txtHIns10,
                    txtHIns11,
                    txtHIns12
                };
                //dodaje zus - zdr do tabeli
                foreach (TextBox item in txtHInsList)
                {
                    dbQuerry = "Update 'tax2019' SET Health=@itemtext where Month=@i";
                    dbCommand = new SQLiteCommand(dbQuerry, dbConnect);         // wysłanie polecenia
                    dbCommand.Parameters.AddWithValue("@itemtext", item.Text.Replace(",", "."));
                    dbCommand.Parameters.AddWithValue("@i", i);
                    dbCommand.ExecuteNonQuery();                                // wykonanie polecenia
                    i++;
                }
                dbConnect.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //
        //to co wyżej tylko odczyt a nie zapis
        //
        private void LoadRev()
        {
            try
            {
                ConectionStrOpen();

                int i = 1;                                              // zmienna dla miesięcy
                //tworzy liste z textboxami kosztów
                List<TextBox> txtGrowRevenueList = new List<TextBox>(12)
                {
                    txtGrowRevenue1,
                    txtGrowRevenue2,
                    txtGrowRevenue3,
                    txtGrowRevenue4,
                    txtGrowRevenue5,
                    txtGrowRevenue6,
                    txtGrowRevenue7,
                    txtGrowRevenue8,
                    txtGrowRevenue9,
                    txtGrowRevenue10,
                    txtGrowRevenue11,
                    txtGrowRevenue12
                };

                foreach (TextBox item in txtGrowRevenueList)
                {
                    dbQuerry = "Select Revenue from 'tax2019' where Month=@i";
                    dbCommand = new SQLiteCommand(dbQuerry, dbConnect);         // wysłanie polecenia
                    dbCommand.Parameters.AddWithValue("@i", i);
                    dbReader = dbCommand.ExecuteReader();

                    while (dbReader.Read())
                    {
                        item.Text = dbReader[0].ToString().Replace(".", ",");
                        i++;
                    }
                }
                dbReader.Close();
                dbConnect.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //wczytuje koszty
        private void LoadCost()
        {
            try
            {
                ConectionStrOpen();

                int i = 1;
                //tworzy liste z textboxami kosztów
                List<TextBox> txtInCostList = new List<TextBox>(12)
                {
                    txtInCost1,
                    txtInCost2,
                    txtInCost3,
                    txtInCost4,
                    txtInCost5,
                    txtInCost6,
                    txtInCost7,
                    txtInCost8,
                    txtInCost9,
                    txtInCost10,
                    txtInCost11,
                    txtInCost12
                };

                foreach (TextBox item in txtInCostList)
                {
                    dbQuerry = "Select Cost from 'tax2019' where Month=@i";
                    dbCommand = new SQLiteCommand(dbQuerry, dbConnect);         // wysłanie polecenia
                    dbCommand.Parameters.AddWithValue("@i", i);
                    dbReader = dbCommand.ExecuteReader();

                    while (dbReader.Read())
                    {
                        item.Text = dbReader[0].ToString().Replace(".", ",");
                        i++;
                    }
                }
                dbReader.Close();
                dbConnect.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadSocial()
        {
            try
            {
                ConectionStrOpen();

                int i = 1;
                //tworzy liste z textboxami kosztów
                List<TextBox> txtSInsList = new List<TextBox>(12)
                {
                    txtSIns1,
                    txtSIns2,
                    txtSIns3,
                    txtSIns4,
                    txtSIns5,
                    txtSIns6,
                    txtSIns7,
                    txtSIns8,
                    txtSIns9,
                    txtSIns10,
                    txtSIns11,
                    txtSIns12
                };

                foreach (TextBox item in txtSInsList)
                {
                    dbQuerry = "Select Social from 'tax2019' where Month=@i";
                    dbCommand = new SQLiteCommand(dbQuerry, dbConnect);         // wysłanie polecenia
                    dbCommand.Parameters.AddWithValue("@i", i);
                    dbReader = dbCommand.ExecuteReader();

                    while (dbReader.Read())
                    {
                        item.Text = dbReader[0].ToString().Replace(".", ",");
                        i++;
                    }
                }
                dbReader.Close();
                dbConnect.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadHealth()
        {
            try
            {
                ConectionStrOpen();

                int i = 1;
                //
                //tworzy liste z textboxami kosztów
                List<TextBox> txtHInsList = new List<TextBox>(12)
                {
                    txtHIns1,
                    txtHIns2,
                    txtHIns3,
                    txtHIns4,
                    txtHIns5,
                    txtHIns6,
                    txtHIns7,
                    txtHIns8,
                    txtHIns9,
                    txtHIns10,
                    txtHIns11,
                    txtHIns12
                };

                foreach (TextBox item in txtHInsList)
                {
                    dbQuerry = "Select Health from 'tax2019' where Month=@i";
                    dbCommand = new SQLiteCommand(dbQuerry, dbConnect);         // wysłanie polecenia
                    dbCommand.Parameters.AddWithValue("@i", i);
                    dbReader = dbCommand.ExecuteReader();

                    while (dbReader.Read())
                    {
                        item.Text = dbReader[0].ToString().Replace(".", ",");
                        i++;
                    }
                }
                dbReader.Close();
                dbConnect.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ZeroIfEmpty()
        {
            if (string.IsNullOrEmpty(txtMonthRevenue10.Text))
            {
                txtMonthRevenue10.Text = "0";
            }
        }

        // ----------------------------------------------------------------------------- METODY DO OBLICZEŃ ---------------------------------

        //suma przychodów
        private void TotalRevenue()
        {
            decimal total = decimal.Parse(txtMonthRevenue1.Text) + decimal.Parse(txtMonthRevenue2.Text) + decimal.Parse(txtMonthRevenue3.Text) +
                decimal.Parse(txtMonthRevenue4.Text) + decimal.Parse(txtMonthRevenue5.Text) + decimal.Parse(txtMonthRevenue6.Text) +
                decimal.Parse(txtMonthRevenue7.Text) + decimal.Parse(txtMonthRevenue8.Text) + decimal.Parse(txtMonthRevenue9.Text) +
                decimal.Parse(txtMonthRevenue10.Text) + decimal.Parse(txtMonthRevenue11.Text) + decimal.Parse(txtMonthRevenue12.Text);

            txtTotalRevenue.Text = total.ToString();
        }

        //suma dochodów
        private void TotalIncome()
        {
            decimal total = decimal.Parse(txtMonthIncome1.Text) + decimal.Parse(txtMonthIncome2.Text) + decimal.Parse(txtMonthIncome3.Text) +
               decimal.Parse(txtMonthIncome4.Text) + decimal.Parse(txtMonthIncome5.Text) + decimal.Parse(txtMonthIncome6.Text) +
               decimal.Parse(txtMonthIncome7.Text) + decimal.Parse(txtMonthIncome8.Text) + decimal.Parse(txtMonthIncome9.Text) +
               decimal.Parse(txtMonthIncome10.Text) + decimal.Parse(txtMonthIncome11.Text) + decimal.Parse(txtMonthIncome12.Text);

            txtTotalIcome.Text = total.ToString();
        }

        //suma kosztów
        private void TotalCosts()
        {
            decimal total =
                decimal.Parse(txtMonthCost1.Text) + decimal.Parse(txtMonthCost2.Text) + decimal.Parse(txtMonthCost3.Text) +
                decimal.Parse(txtMonthCost4.Text) + decimal.Parse(txtMonthCost5.Text) + decimal.Parse(txtMonthCost6.Text) +
                decimal.Parse(txtMonthCost7.Text) + decimal.Parse(txtMonthCost8.Text) + decimal.Parse(txtMonthCost9.Text) +
                decimal.Parse(txtMonthCost10.Text) + decimal.Parse(txtMonthCost11.Text) + decimal.Parse(txtMonthCost12.Text);

            txtTotalCost.Text = total.ToString();
        }

        //suma składek społecznych
        private void TotalSocialIns()
        {
            decimal total =
                decimal.Parse(txtSIns1.Text) + decimal.Parse(txtSIns2.Text) + decimal.Parse(txtSIns3.Text) +
                decimal.Parse(txtSIns4.Text) + decimal.Parse(txtSIns5.Text) + decimal.Parse(txtSIns6.Text) +
                decimal.Parse(txtSIns7.Text) + decimal.Parse(txtSIns8.Text) + decimal.Parse(txtSIns9.Text) +
                decimal.Parse(txtSIns10.Text) + decimal.Parse(txtSIns11.Text) + decimal.Parse(txtSIns12.Text);

            txtTotalSIns.Text = total.ToString();
        }

        //suma składek zdrowotnych
        private void TotalHealthIns()
        {
            decimal total =
                decimal.Parse(txtHIns1.Text) + decimal.Parse(txtHIns2.Text) + decimal.Parse(txtHIns3.Text) +
                decimal.Parse(txtHIns4.Text) + decimal.Parse(txtHIns5.Text) + decimal.Parse(txtHIns6.Text) +
                decimal.Parse(txtHIns7.Text) + decimal.Parse(txtHIns8.Text) + decimal.Parse(txtHIns9.Text) +
                decimal.Parse(txtHIns10.Text) + decimal.Parse(txtHIns11.Text) + decimal.Parse(txtHIns12.Text);

            txtTotallHIns.Text = total.ToString();
        }

        //suma ZdrOdl
        private void TotalAmountDeductible()
        {
            decimal total =
                decimal.Parse(txtAmountDed1.Text) + decimal.Parse(txtAmountDed2.Text) + decimal.Parse(txtAmountDed3.Text) +
                decimal.Parse(txtAmountDed4.Text) + decimal.Parse(txtAmountDed5.Text) + decimal.Parse(txtAmountDed6.Text) +
                decimal.Parse(txtAmountDed7.Text) + decimal.Parse(txtAmountDed8.Text) + decimal.Parse(txtAmountDed9.Text) +
                decimal.Parse(txtAmountDed10.Text) + decimal.Parse(txtAmountDed11.Text) + decimal.Parse(txtAmountDed12.Text);

            txtTotalAmountDed.Text = total.ToString();
        }

        //suma zaliczek na podatek
        private void TotalTaxPrepayment()
        {
            decimal total =
                decimal.Parse(txtTaxPrepay1.Text) + decimal.Parse(txtTaxPrepay2.Text) + decimal.Parse(txtTaxPrepay3.Text) +
                decimal.Parse(txtTaxPrepay4.Text) + decimal.Parse(txtTaxPrepay5.Text) + decimal.Parse(txtTaxPrepay6.Text) +
                decimal.Parse(txtTaxPrepay7.Text) + decimal.Parse(txtTaxPrepay8.Text) + decimal.Parse(txtTaxPrepay9.Text) +
                decimal.Parse(txtTaxPrepay10.Text) + decimal.Parse(txtTaxPrepay11.Text) + decimal.Parse(txtTaxPrepay12.Text);

            txtTotalTaxPrepay.Text = total.ToString();
        }

        //pokazuje zsumowane wszystkie pola
        private void ShowTotal()
        {
            TotalRevenue();
            TotalIncome();
            TotalCosts();
            TotalSocialIns();
            TotalHealthIns();
            TotalAmountDeductible();
            TotalTaxPrepayment();
        }

        //wczytuje wszystko z bazy danych
        private void LoadAllFromDB()
        {
            LoadRev();
            LoadCost();
            LoadSocial();
            LoadHealth();
            timer1.Start();
            lblInfo.Text = "... wczytano dane dane z bazy danych ...";
        }

        //Oblicza ZdrOdl - czyli wartość do odliczenia z podatku od zusu
        private decimal CalculateAmountDed(decimal social)
        {
            return social / 9.0m * 7.75m;
        }

        // tak - wiem.  ;]
        private decimal Calculate(decimal month1, decimal month2)
        {
            return month1 - month2;
        }

        //Oblicza zaliczkę na podatek
        private decimal CalculateTaxPrepayment(decimal revenue, decimal cost, decimal social, decimal amountDed)
        {
            if ((((revenue - cost - social) * taxRate) - amountDed) > 0)
            {
                return ((revenue - cost - social) * taxRate) - amountDed;
            }
            else
            {
                return 0;
            }
        }


        private decimal CheckPayment(decimal tpp, decimal p)
        {
            if (p == tpp)
            {
                return 0.00m;
            }
            else if (p < tpp)
            {
                return p - tpp;
            }
            else
            {
                return p - tpp;
            }
        }

        // ------------------------------------------------------------------------------- RATY
        //------------------------------------------------------------------------------------------------------------------Oblicza: 1 rata
        private void TaxPrepayMonth1()
        {
            try
            {
                if (decimal.Parse(txtGrowRevenue1.Text) >= 0 && decimal.Parse(txtInCost1.Text) >= 0)
                {
                    txtAmountDed1.Text = CalculateAmountDed(decimal.Parse(txtHIns1.Text)).ToString("n2");
                    decimal taxP, amD, SIns;
                    amD = CalculateAmountDed(decimal.Parse(txtHIns1.Text));
                    taxP = CalculateTaxPrepayment(decimal.Parse(txtMonthRevenue1.Text), decimal.Parse(txtMonthCost1.Text), decimal.Parse(txtSIns1.Text), amD);
                    txtTaxPrepay1.Text = taxP.ToString("n0");

                    SIns = decimal.Parse(txtSIns1.Text);

                    taxP = CalculateTaxPrepayment(decimal.Parse(txtMonthRevenue1.Text), decimal.Parse(txtMonthCost1.Text), SIns, amD);
                    txtAmountDed2.Text = CalculateAmountDed(decimal.Parse(txtHIns1.Text)).ToString("n2");

                    txtTaxPrepay2.Text = taxP.ToString("n0");

                    txtMonthRevenue1.Text = decimal.Parse(txtGrowRevenue1.Text).ToString("n2");
                    txtMonthCost1.Text = decimal.Parse(txtInCost1.Text).ToString("n2");
                    txtMonthIncome1.Text = Calculate(decimal.Parse(txtGrowRevenue1.Text), decimal.Parse(txtInCost1.Text)).ToString();

                    ShowTotal();
                }
                //jeżeli usniemy / wyzerujemy przychood i koszty to zeruje nam odpowiednie textboxy
                else if (decimal.Parse(txtGrowRevenue1.Text) == 0 || decimal.Parse(txtGrowRevenue1.Text) == 0)
                {
                    txtMonthRevenue1.Text = "0,00";
                    txtMonthCost1.Text = "0,00";
                    txtSIns1.Text = "0,00";
                    txtHIns1.Text = "0,00";
                    txtMonthIncome1.Text = "0,00";
                    txtAmountDed1.Text = "0,00";
                    txtTaxPrepay1.Text = "0,00";

                    ShowTotal();
                }
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - [styczeń]";
            }
        }
        //------------------------------------------------------------------------------------------------------------------Oblicza: 2 rata
        private void TaxPrepayMonth2()
        {
            try
            {
                // taxP - podatek od calosci - trzeba odjąć wszystkie wplacone raty !!!
                // amD .. amD2 - jest to skladka zdrowotna, którą się odlicz od podatku
                // tPay1 ... tPay12 - zsumowane wpłacone raty z poprzednich m-cy, które odejmiemy od taxP (od całości podatku)

                decimal tPay1;
                decimal taxP, amD, amD1, amD2;
                decimal SIns, SIns1, SIns2;

                //przychód (narastający!) za dany misiąc musi buć równy lub większy od porzedniego misiąca i większy od zera które jest przypisane na starcie
                if (decimal.Parse(txtGrowRevenue2.Text) >= decimal.Parse(txtGrowRevenue1.Text) && decimal.Parse(txtGrowRevenue2.Text) > 0)
                {
                    SIns1 = decimal.Parse(txtSIns1.Text);
                    SIns2 = decimal.Parse(txtSIns2.Text);

                    SIns = SIns1 + SIns2;

                    amD1 = CalculateAmountDed(decimal.Parse(txtHIns1.Text));
                    amD2 = CalculateAmountDed(decimal.Parse(txtHIns2.Text));

                    amD = amD1 + amD2;

                    taxP = CalculateTaxPrepayment(decimal.Parse(txtMonthRevenue2.Text), decimal.Parse(txtMonthCost2.Text), SIns, amD);
                    txtAmountDed2.Text = CalculateAmountDed(decimal.Parse(txtHIns2.Text)).ToString("n2");

                    tPay1 = decimal.Parse(txtTaxPrepay1.Text);

                    taxP = taxP - tPay1; // odejmuje wpłacone zaliczki od podatku

                    txtTaxPrepay2.Text = taxP.ToString("n0");

                    txtMonthRevenue2.Text = Calculate(decimal.Parse(txtGrowRevenue2.Text), decimal.Parse(txtGrowRevenue1.Text)).ToString("n2");
                    txtMonthIncome2.Text = Calculate(decimal.Parse(txtMonthRevenue2.Text), decimal.Parse(txtMonthCost2.Text)).ToString();
                    txtMonthCost2.Text = Calculate(decimal.Parse(txtInCost2.Text), decimal.Parse(txtInCost1.Text)).ToString("n2");

                    ShowTotal();
                }
                //jeżeli usniemy / wyzerujemy przychood i koszty to zeruje nam odpowiednie textboxy
                else if (decimal.Parse(txtGrowRevenue2.Text) == 0 || decimal.Parse(txtGrowRevenue2.Text) == 0)
                {
                    txtMonthRevenue2.Text = "0,00";
                    txtMonthCost2.Text = "0,00";
                    txtSIns2.Text = "0,00";
                    txtHIns2.Text = "0,00";
                    txtMonthIncome2.Text = "0,00";
                    txtAmountDed2.Text = "0,00";
                    txtTaxPrepay2.Text = "0,00";
                    ShowTotal();
                }
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - [luty]";
            }
        }
        //------------------------------------------------------------------------------------------------------------------Oblicza: 3 rata
        private void TaxPrepayMonth3()
        {
            try
            {
                // taxP - podatek od calosci - trzeba odjąć wszystkie wplacone raty !!!
                // amD .. amD2 - jest to skladka zdrowotna, którą się odlicz od podatku
                // tPay1 ... tPay12 - zsumowane wpłacone raty z poprzednich m-cy, które odejmiemy od taxP (od całości podatku)

                decimal tPay1, tPay2;
                decimal taxP, amD, amD1, amD2, amD3;
                decimal SIns, SIns1, SIns2, SIns3;

                //przychód (narastający!) za dany misiąc musi buć równy lub większy od porzedniego misiąca i większy od zera które jest przypisane na starcie
                if (decimal.Parse(txtGrowRevenue3.Text) >= decimal.Parse(txtGrowRevenue2.Text) && decimal.Parse(txtGrowRevenue3.Text) > 0)
                {
                    SIns1 = decimal.Parse(txtSIns1.Text);
                    SIns2 = decimal.Parse(txtSIns2.Text);
                    SIns3 = decimal.Parse(txtSIns3.Text);

                    SIns = SIns1 + SIns2 + SIns3;

                    amD1 = CalculateAmountDed(decimal.Parse(txtHIns1.Text));
                    amD2 = CalculateAmountDed(decimal.Parse(txtHIns2.Text));
                    amD3 = CalculateAmountDed(decimal.Parse(txtHIns3.Text));

                    amD = amD1 + amD2 + amD3;

                    taxP = CalculateTaxPrepayment(decimal.Parse(txtGrowRevenue3.Text), decimal.Parse(txtInCost3.Text), SIns, amD); //NARASTAJĄCY PRZYCHÓD !!!!
                    txtAmountDed3.Text = CalculateAmountDed(decimal.Parse(txtHIns3.Text)).ToString("n2");

                    tPay1 = decimal.Parse(txtTaxPrepay1.Text);
                    tPay2 = decimal.Parse(txtTaxPrepay2.Text);

                    taxP = taxP - tPay1 - tPay2; // odejmuje wpłacone zaliczki od podatku

                    txtTaxPrepay3.Text = taxP.ToString("n0");

                    txtMonthRevenue3.Text = Calculate(decimal.Parse(txtGrowRevenue3.Text), decimal.Parse(txtGrowRevenue2.Text)).ToString("n2");
                    txtMonthIncome3.Text = Calculate(decimal.Parse(txtMonthRevenue3.Text), decimal.Parse(txtMonthCost3.Text)).ToString();
                    txtMonthCost3.Text = Calculate(decimal.Parse(txtInCost3.Text), decimal.Parse(txtInCost2.Text)).ToString("n2");

                    ShowTotal();
                }
                //jeżeli usniemy / wyzerujemy przychood i koszty to zeruje nam odpowiednie textboxy
                else if (decimal.Parse(txtGrowRevenue3.Text) == 0 || decimal.Parse(txtGrowRevenue3.Text) == 0)
                {
                    txtMonthRevenue3.Text = "0,00";
                    txtMonthCost3.Text = "0,00";
                    txtSIns3.Text = "0,00";
                    txtHIns3.Text = "0,00";
                    txtMonthIncome3.Text = "0,00";
                    txtAmountDed3.Text = "0,00";
                    txtTaxPrepay3.Text = "0,00";

                    ShowTotal();
                }

            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - [marzec]";
            }
        }
        //------------------------------------------------------------------------------------------------------------------Oblicza: 7 rata
        private void TaxPrepayMonth4()
        {
            try
            {
                // taxP - podatek od calosci - trzeba odjąć wszystkie wplacone raty !!!
                // amD .. amD2 - jest to skladka zdrowotna, którą się odlicz od podatku
                // tPay1 ... tPay12 - zsumowane wpłacone raty z poprzednich m-cy, które odejmiemy od taxP (od całości podatku)

                decimal tPay1, tPay2, tPay3;
                decimal taxP, amD, amD1, amD2, amD3, amD4;
                decimal SIns, SIns1, SIns2, SIns3, SIns4;

                //przychód (narastający!) za dany misiąc musi buć równy lub większy od porzedniego misiąca i większy od zera które jest przypisane na starcie
                if (decimal.Parse(txtGrowRevenue4.Text) >= decimal.Parse(txtGrowRevenue3.Text) && decimal.Parse(txtGrowRevenue4.Text) > 0)
                {
                    SIns1 = decimal.Parse(txtSIns1.Text);
                    SIns2 = decimal.Parse(txtSIns2.Text);
                    SIns3 = decimal.Parse(txtSIns3.Text);
                    SIns4 = decimal.Parse(txtSIns4.Text);

                    SIns = SIns1 + SIns2 + SIns3 + SIns4;

                    amD1 = CalculateAmountDed(decimal.Parse(txtHIns1.Text));
                    amD2 = CalculateAmountDed(decimal.Parse(txtHIns2.Text));
                    amD3 = CalculateAmountDed(decimal.Parse(txtHIns3.Text));
                    amD4 = CalculateAmountDed(decimal.Parse(txtHIns4.Text));

                    amD = amD1 + amD2 + amD3 + amD4;

                    taxP = CalculateTaxPrepayment(decimal.Parse(txtGrowRevenue4.Text), decimal.Parse(txtInCost4.Text), SIns, amD); //NARASTAJĄCY PRZYCHÓD !!!!
                    txtAmountDed4.Text = CalculateAmountDed(decimal.Parse(txtHIns4.Text)).ToString("n2");

                    tPay1 = decimal.Parse(txtTaxPrepay1.Text);
                    tPay2 = decimal.Parse(txtTaxPrepay2.Text);
                    tPay3 = decimal.Parse(txtTaxPrepay3.Text);

                    taxP = taxP - tPay1 - tPay2 - tPay3; // odejmuje wpłacone zaliczki od podatku

                    txtTaxPrepay4.Text = taxP.ToString("n0");

                    txtMonthRevenue4.Text = Calculate(decimal.Parse(txtGrowRevenue4.Text), decimal.Parse(txtGrowRevenue3.Text)).ToString("n2");
                    txtMonthIncome4.Text = Calculate(decimal.Parse(txtMonthRevenue4.Text), decimal.Parse(txtMonthCost4.Text)).ToString();
                    txtMonthCost4.Text = Calculate(decimal.Parse(txtInCost4.Text), decimal.Parse(txtInCost3.Text)).ToString("n2");

                    ShowTotal();
                }
                //jeżeli usniemy / wyzerujemy przychood i koszty to zeruje nam odpowiednie textboxy
                else if (decimal.Parse(txtGrowRevenue4.Text) == 0 || decimal.Parse(txtGrowRevenue4.Text) == 0)
                {
                    txtMonthRevenue4.Text = "0,00";
                    txtMonthCost4.Text = "0,00";
                    txtSIns4.Text = "0,00";
                    txtHIns4.Text = "0,00";
                    txtMonthIncome4.Text = "0,00";
                    txtAmountDed4.Text = "0,00";
                    txtTaxPrepay4.Text = "0,00";

                    ShowTotal();
                }
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - [kwiecień]";
            }
        }
        //------------------------------------------------------------------------------------------------------------------Oblicza: 5 rata
        private void TaxPrepayMonth5()
        {
            try
            {
                // taxP - podatek od calosci - trzeba odjąć wszystkie wplacone raty !!!
                // amD .. amD2 - jest to skladka zdrowotna, którą się odlicz od podatku
                // tPay1 ... tPay12 - zsumowane wpłacone raty z poprzednich m-cy, które odejmiemy od taxP (od całości podatku)

                decimal tPay1, tPay2, tPay3, tPay4;
                decimal taxP, amD, amD1, amD2, amD3, amD4, amD5;
                decimal SIns, SIns1, SIns2, SIns3, SIns4, SIns5;

                //przychód (narastający!) za dany misiąc musi buć równy lub większy od porzedniego misiąca i większy od zera które jest przypisane na starcie
                if (decimal.Parse(txtGrowRevenue5.Text) >= decimal.Parse(txtGrowRevenue7.Text) && decimal.Parse(txtGrowRevenue5.Text) > 0)
                {
                    SIns1 = decimal.Parse(txtSIns1.Text);
                    SIns2 = decimal.Parse(txtSIns2.Text);
                    SIns3 = decimal.Parse(txtSIns3.Text);
                    SIns4 = decimal.Parse(txtSIns4.Text);
                    SIns5 = decimal.Parse(txtSIns5.Text);

                    SIns = SIns1 + SIns2 + SIns3 + SIns4 + SIns5;

                    amD1 = CalculateAmountDed(decimal.Parse(txtHIns1.Text));
                    amD2 = CalculateAmountDed(decimal.Parse(txtHIns2.Text));
                    amD3 = CalculateAmountDed(decimal.Parse(txtHIns3.Text));
                    amD4 = CalculateAmountDed(decimal.Parse(txtHIns4.Text));
                    amD5 = CalculateAmountDed(decimal.Parse(txtHIns5.Text));

                    amD = amD1 + amD2 + amD3 + amD4 + amD5;

                    taxP = CalculateTaxPrepayment(decimal.Parse(txtGrowRevenue5.Text), decimal.Parse(txtInCost5.Text), SIns, amD); //NARASTAJĄCY PRZYCHÓD !!!!
                    txtAmountDed5.Text = CalculateAmountDed(decimal.Parse(txtHIns5.Text)).ToString("n2");

                    tPay1 = decimal.Parse(txtTaxPrepay1.Text);
                    tPay2 = decimal.Parse(txtTaxPrepay2.Text);
                    tPay3 = decimal.Parse(txtTaxPrepay3.Text);
                    tPay4 = decimal.Parse(txtTaxPrepay4.Text);

                    taxP = taxP - tPay1 - tPay2 - tPay3 - tPay4; // odejmuje wpłacone zaliczki od podatku

                    txtTaxPrepay5.Text = taxP.ToString("n0");

                    txtMonthRevenue5.Text = Calculate(decimal.Parse(txtGrowRevenue5.Text), decimal.Parse(txtGrowRevenue4.Text)).ToString("n2");
                    txtMonthIncome5.Text = Calculate(decimal.Parse(txtMonthRevenue5.Text), decimal.Parse(txtMonthCost5.Text)).ToString();
                    txtMonthCost5.Text = Calculate(decimal.Parse(txtInCost5.Text), decimal.Parse(txtInCost4.Text)).ToString("n2");

                    ShowTotal();
                }
                //jeżeli usniemy / wyzerujemy przychood i koszty to zeruje nam odpowiednie textboxy
                else if (decimal.Parse(txtGrowRevenue5.Text) == 0 || decimal.Parse(txtGrowRevenue5.Text) == 0)
                {
                    txtMonthRevenue5.Text = "0,00";
                    txtMonthCost5.Text = "0,00";
                    txtSIns5.Text = "0,00";
                    txtHIns5.Text = "0,00";
                    txtMonthIncome5.Text = "0,00";
                    txtAmountDed5.Text = "0,00";
                    txtTaxPrepay5.Text = "0,00";
                    ShowTotal();
                }
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - [maj]";
            }
        }
        //------------------------------------------------------------------------------------------------------------------Oblicza: 6 rata
        private void TaxPrepayMonth6()
        {
            try
            {
                // taxP - podatek od calosci - trzeba odjąć wszystkie wplacone raty !!!
                // amD .. amD2 - jest to skladka zdrowotna, którą się odlicz od podatku
                // tPay1 ... tPay12 - zsumowane wpłacone raty z poprzednich m-cy, które odejmiemy od taxP (od całości podatku)

                decimal tPay1, tPay2, tPay3, tPay4, tPay5;
                decimal taxP, amD, amD1, amD2, amD3, amD4, amD5, amD6;
                decimal SIns, SIns1, SIns2, SIns3, SIns4, SIns5, SIns6;

                //przychód (narastający!) za dany misiąc musi buć równy lub większy od porzedniego misiąca i większy od zera które jest przypisane na starcie
                if (decimal.Parse(txtGrowRevenue6.Text) >= decimal.Parse(txtGrowRevenue5.Text) && decimal.Parse(txtGrowRevenue6.Text) > 0)
                {
                    SIns1 = decimal.Parse(txtSIns1.Text);
                    SIns2 = decimal.Parse(txtSIns2.Text);
                    SIns3 = decimal.Parse(txtSIns3.Text);
                    SIns4 = decimal.Parse(txtSIns4.Text);
                    SIns5 = decimal.Parse(txtSIns5.Text);
                    SIns6 = decimal.Parse(txtSIns6.Text);

                    SIns = SIns1 + SIns2 + SIns3 + SIns4 + SIns5 + SIns6;

                    amD1 = CalculateAmountDed(decimal.Parse(txtHIns1.Text));
                    amD2 = CalculateAmountDed(decimal.Parse(txtHIns2.Text));
                    amD3 = CalculateAmountDed(decimal.Parse(txtHIns3.Text));
                    amD4 = CalculateAmountDed(decimal.Parse(txtHIns4.Text));
                    amD5 = CalculateAmountDed(decimal.Parse(txtHIns5.Text));
                    amD6 = CalculateAmountDed(decimal.Parse(txtHIns6.Text));

                    amD = amD1 + amD2 + amD3 + amD4 + amD5 + amD6;

                    taxP = CalculateTaxPrepayment(decimal.Parse(txtGrowRevenue6.Text), decimal.Parse(txtInCost6.Text), SIns, amD); //NARASTAJĄCY PRZYCHÓD !!!!
                    txtAmountDed6.Text = CalculateAmountDed(decimal.Parse(txtHIns6.Text)).ToString("n2");

                    tPay1 = decimal.Parse(txtTaxPrepay1.Text);
                    tPay2 = decimal.Parse(txtTaxPrepay2.Text);
                    tPay3 = decimal.Parse(txtTaxPrepay3.Text);
                    tPay4 = decimal.Parse(txtTaxPrepay4.Text);
                    tPay5 = decimal.Parse(txtTaxPrepay5.Text);

                    taxP = taxP - tPay1 - tPay2 - tPay3 - tPay4 - tPay5; // odejmuje wpłacone zaliczki od podatku

                    txtTaxPrepay6.Text = taxP.ToString("n0");

                    txtMonthRevenue6.Text = Calculate(decimal.Parse(txtGrowRevenue6.Text), decimal.Parse(txtGrowRevenue5.Text)).ToString("n2");
                    txtMonthIncome6.Text = Calculate(decimal.Parse(txtMonthRevenue6.Text), decimal.Parse(txtMonthCost6.Text)).ToString();
                    txtMonthCost6.Text = Calculate(decimal.Parse(txtInCost6.Text), decimal.Parse(txtInCost5.Text)).ToString("n2");

                    ShowTotal();
                }
                //jeżeli usniemy / wyzerujemy przychood i koszty to zeruje nam odpowiednie textboxy
                else if (decimal.Parse(txtGrowRevenue6.Text) == 0 || decimal.Parse(txtGrowRevenue6.Text) == 0)
                {
                    txtMonthRevenue6.Text = "0,00";
                    txtMonthCost6.Text = "0,00";
                    txtSIns6.Text = "0,00";
                    txtHIns6.Text = "0,00";
                    txtMonthIncome6.Text = "0,00";
                    txtAmountDed6.Text = "0,00";
                    txtTaxPrepay6.Text = "0,00";

                    ShowTotal();
                }

            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - [czerwiec]";
            }
        }
        //------------------------------------------------------------------------------------------------------------------Oblicza: 7 rata
        private void TaxPrepayMonth7()
        {
            try
            {
                // taxP - podatek od calosci - trzeba odjąć wszystkie wplacone raty !!!
                // amD .. amD2 - jest to skladka zdrowotna, którą się odlicz od podatku
                // tPay1 ... tPay12 - zsumowane wpłacone raty z poprzednich m-cy, które odejmiemy od taxP (od całości podatku)

                //przychód (narastający!) za dany misiąc musi buć równy lub większy od porzedniego misiąca i większy od zera które jest przypisane na starcie
                if (decimal.Parse(txtGrowRevenue7.Text) >= decimal.Parse(txtGrowRevenue6.Text) && decimal.Parse(txtGrowRevenue7.Text) > 0)
                {
                    decimal tPay1, tPay2, tPay3, tPay4, tPay5, tPay6;
                    decimal taxP, amD, amD1, amD2, amD3, amD4, amD5, amD6, amD7;
                    decimal SIns, SIns1, SIns2, SIns3, SIns4, SIns5, SIns6, SIns7;

                    SIns1 = decimal.Parse(txtSIns1.Text);
                    SIns2 = decimal.Parse(txtSIns2.Text);
                    SIns3 = decimal.Parse(txtSIns3.Text);
                    SIns4 = decimal.Parse(txtSIns4.Text);
                    SIns5 = decimal.Parse(txtSIns5.Text);
                    SIns6 = decimal.Parse(txtSIns6.Text);
                    SIns7 = decimal.Parse(txtSIns7.Text);

                    SIns = SIns1 + SIns2 + SIns3 + SIns4 + SIns5 + SIns6 + SIns7;

                    amD1 = CalculateAmountDed(decimal.Parse(txtHIns1.Text));
                    amD2 = CalculateAmountDed(decimal.Parse(txtHIns2.Text));
                    amD3 = CalculateAmountDed(decimal.Parse(txtHIns3.Text));
                    amD4 = CalculateAmountDed(decimal.Parse(txtHIns4.Text));
                    amD5 = CalculateAmountDed(decimal.Parse(txtHIns5.Text));
                    amD6 = CalculateAmountDed(decimal.Parse(txtHIns6.Text));
                    amD7 = CalculateAmountDed(decimal.Parse(txtHIns7.Text));

                    amD = amD1 + amD2 + amD3 + amD4 + amD5 + amD6 + amD7;

                    taxP = CalculateTaxPrepayment(decimal.Parse(txtGrowRevenue7.Text), decimal.Parse(txtInCost7.Text), SIns, amD); //NARASTAJĄCY PRZYCHÓD !!!!
                    txtAmountDed7.Text = CalculateAmountDed(decimal.Parse(txtHIns7.Text)).ToString("n2");

                    tPay1 = decimal.Parse(txtTaxPrepay1.Text);
                    tPay2 = decimal.Parse(txtTaxPrepay2.Text);
                    tPay3 = decimal.Parse(txtTaxPrepay3.Text);
                    tPay4 = decimal.Parse(txtTaxPrepay4.Text);
                    tPay5 = decimal.Parse(txtTaxPrepay5.Text);
                    tPay6 = decimal.Parse(txtTaxPrepay6.Text);

                    taxP = taxP - tPay1 - tPay2 - tPay3 - tPay4 - tPay5 - tPay6; // odejmuje wpłacone zaliczki od podatku

                    txtTaxPrepay7.Text = taxP.ToString("n0");

                    txtMonthRevenue7.Text = Calculate(decimal.Parse(txtGrowRevenue7.Text), decimal.Parse(txtGrowRevenue6.Text)).ToString("n2");
                    txtMonthIncome7.Text = Calculate(decimal.Parse(txtMonthRevenue7.Text), decimal.Parse(txtMonthCost7.Text)).ToString();
                    txtMonthCost7.Text = Calculate(decimal.Parse(txtInCost7.Text), decimal.Parse(txtInCost6.Text)).ToString("n2");

                    ShowTotal();
                }
                //jeżeli usniemy / wyzerujemy przychood i koszty to zeruje nam odpowiednie textboxy
                else if (decimal.Parse(txtGrowRevenue7.Text) == 0 || decimal.Parse(txtGrowRevenue7.Text) == 0)
                {
                    txtMonthRevenue7.Text = "0,00";
                    txtMonthCost7.Text = "0,00";
                    txtSIns7.Text = "0,00";
                    txtHIns7.Text = "0,00";
                    txtMonthIncome7.Text = "0,00";
                    txtAmountDed7.Text = "0,00";
                    txtTaxPrepay7.Text = "0,00";
                    ShowTotal();
                }
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - [lipiec]";
            }
        }
        //------------------------------------------------------------------------------------------------------------------Oblicza: 8 rata
        private void TaxPrepayMonth8()
        {
            try
            {
                // taxP - podatek od calosci - trzeba odjąć wszystkie wplacone raty !!!
                // amD .. amD2 - jest to skladka zdrowotna, którą się odlicz od podatku
                // tPay1 ... tPay12 - zsumowane wpłacone raty z poprzednich m-cy, które odejmiemy od taxP (od całości podatku)

                decimal tPay1, tPay2, tPay3, tPay4, tPay5, tPay6, tPay7;
                decimal taxP, amD, amD1, amD2, amD3, amD4, amD5, amD6, amD7, amD8;
                decimal SIns, SIns1, SIns2, SIns3, SIns4, SIns5, SIns6, SIns7, SIns8;

                //przychód (narastający!) za dany misiąc musi buć równy lub większy od porzedniego misiąca i większy od zera które jest przypisane na starcie
                if (decimal.Parse(txtGrowRevenue8.Text) >= decimal.Parse(txtGrowRevenue7.Text) && decimal.Parse(txtGrowRevenue8.Text) > 0)
                {
                    SIns1 = decimal.Parse(txtSIns1.Text);
                    SIns2 = decimal.Parse(txtSIns2.Text);
                    SIns3 = decimal.Parse(txtSIns3.Text);
                    SIns4 = decimal.Parse(txtSIns4.Text);
                    SIns5 = decimal.Parse(txtSIns5.Text);
                    SIns6 = decimal.Parse(txtSIns6.Text);
                    SIns7 = decimal.Parse(txtSIns7.Text);
                    SIns8 = decimal.Parse(txtSIns8.Text);

                    SIns = SIns1 + SIns2 + SIns3 + SIns4 + SIns5 + SIns6 + SIns7 + SIns8;

                    amD1 = CalculateAmountDed(decimal.Parse(txtHIns1.Text));
                    amD2 = CalculateAmountDed(decimal.Parse(txtHIns2.Text));
                    amD3 = CalculateAmountDed(decimal.Parse(txtHIns3.Text));
                    amD4 = CalculateAmountDed(decimal.Parse(txtHIns4.Text));
                    amD5 = CalculateAmountDed(decimal.Parse(txtHIns5.Text));
                    amD6 = CalculateAmountDed(decimal.Parse(txtHIns6.Text));
                    amD7 = CalculateAmountDed(decimal.Parse(txtHIns7.Text));
                    amD8 = CalculateAmountDed(decimal.Parse(txtHIns8.Text));

                    amD = amD1 + amD2 + amD3 + amD4 + amD5 + amD6 + amD7 + amD8;

                    taxP = CalculateTaxPrepayment(decimal.Parse(txtGrowRevenue8.Text), decimal.Parse(txtInCost8.Text), SIns, amD); //NARASTAJĄCY PRZYCHÓD !!!!
                    txtAmountDed8.Text = CalculateAmountDed(decimal.Parse(txtHIns8.Text)).ToString("n2");

                    tPay1 = decimal.Parse(txtTaxPrepay1.Text);
                    tPay2 = decimal.Parse(txtTaxPrepay2.Text);
                    tPay3 = decimal.Parse(txtTaxPrepay3.Text);
                    tPay4 = decimal.Parse(txtTaxPrepay4.Text);
                    tPay5 = decimal.Parse(txtTaxPrepay5.Text);
                    tPay6 = decimal.Parse(txtTaxPrepay6.Text);
                    tPay7 = decimal.Parse(txtTaxPrepay7.Text);

                    taxP = taxP - tPay1 - tPay2 - tPay3 - tPay4 - tPay5 - tPay6 - tPay7; // odejmuje wpłacone zaliczki od podatku

                    txtTaxPrepay8.Text = taxP.ToString("n0");

                    txtMonthRevenue8.Text = Calculate(decimal.Parse(txtGrowRevenue8.Text), decimal.Parse(txtGrowRevenue7.Text)).ToString("n2");
                    txtMonthIncome8.Text = Calculate(decimal.Parse(txtMonthRevenue8.Text), decimal.Parse(txtMonthCost8.Text)).ToString();
                    txtMonthCost8.Text = Calculate(decimal.Parse(txtInCost8.Text), decimal.Parse(txtInCost7.Text)).ToString("n2");

                    ShowTotal();
                }
                //jeżeli usniemy / wyzerujemy przychood i koszty to zeruje nam odpowiednie textboxy
                else if (decimal.Parse(txtGrowRevenue8.Text) == 0 || decimal.Parse(txtGrowRevenue8.Text) == 0)
                {
                    txtMonthRevenue8.Text = "0,00";
                    txtMonthCost8.Text = "0,00";
                    txtSIns8.Text = "0,00";
                    txtHIns8.Text = "0,00";
                    txtMonthIncome8.Text = "0,00";
                    txtAmountDed8.Text = "0,00";
                    txtTaxPrepay8.Text = "0,00";

                    ShowTotal();
                }
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - [sierpień]";
            }
        }
        //------------------------------------------------------------------------------------------------------------------Oblicza: 9 rata
        private void TaxPrepayMonth9()
        {
            try
            {
                // taxP - podatek od calosci - trzeba odjąć wszystkie wplacone raty !!!
                // amD .. amD2 - jest to skladka zdrowotna, którą się odlicz od podatku
                // tPay1 ... tPay12 - zsumowane wpłacone raty z poprzednich m-cy, które odejmiemy od taxP (od całości podatku)

                decimal tPay1, tPay2, tPay3, tPay4, tPay5, tPay6, tPay7, tPay8;
                decimal taxP, amD, amD1, amD2, amD3, amD4, amD5, amD6, amD7, amD8, amD9;
                decimal SIns, SIns1, SIns2, SIns3, SIns4, SIns5, SIns6, SIns7, SIns8, SIns9;

                //przychód (narastający!) za dany misiąc musi buć równy lub większy od porzedniego misiąca i większy od zera które jest przypisane na starcie
                if (decimal.Parse(txtGrowRevenue9.Text) >= decimal.Parse(txtGrowRevenue8.Text) && decimal.Parse(txtGrowRevenue9.Text) > 0)
                {
                    SIns1 = decimal.Parse(txtSIns1.Text);
                    SIns2 = decimal.Parse(txtSIns2.Text);
                    SIns3 = decimal.Parse(txtSIns3.Text);
                    SIns4 = decimal.Parse(txtSIns4.Text);
                    SIns5 = decimal.Parse(txtSIns5.Text);
                    SIns6 = decimal.Parse(txtSIns6.Text);
                    SIns7 = decimal.Parse(txtSIns7.Text);
                    SIns8 = decimal.Parse(txtSIns8.Text);
                    SIns9 = decimal.Parse(txtSIns9.Text);

                    SIns = SIns1 + SIns2 + SIns3 + SIns4 + SIns5 + SIns6 + SIns7 + SIns8 + SIns9;

                    amD1 = CalculateAmountDed(decimal.Parse(txtHIns1.Text));
                    amD2 = CalculateAmountDed(decimal.Parse(txtHIns2.Text));
                    amD3 = CalculateAmountDed(decimal.Parse(txtHIns3.Text));
                    amD4 = CalculateAmountDed(decimal.Parse(txtHIns4.Text));
                    amD5 = CalculateAmountDed(decimal.Parse(txtHIns5.Text));
                    amD6 = CalculateAmountDed(decimal.Parse(txtHIns6.Text));
                    amD7 = CalculateAmountDed(decimal.Parse(txtHIns7.Text));
                    amD8 = CalculateAmountDed(decimal.Parse(txtHIns8.Text));
                    amD9 = CalculateAmountDed(decimal.Parse(txtHIns9.Text));

                    amD = amD1 + amD2 + amD3 + amD4 + amD5 + amD6 + amD7 + amD8 + amD9;

                    taxP = CalculateTaxPrepayment(decimal.Parse(txtGrowRevenue9.Text), decimal.Parse(txtInCost9.Text), SIns, amD); //NARASTAJĄCY PRZYCHÓD !!!!
                    txtAmountDed9.Text = CalculateAmountDed(decimal.Parse(txtHIns9.Text)).ToString("n2");

                    tPay1 = decimal.Parse(txtTaxPrepay1.Text);
                    tPay2 = decimal.Parse(txtTaxPrepay2.Text);
                    tPay3 = decimal.Parse(txtTaxPrepay3.Text);
                    tPay4 = decimal.Parse(txtTaxPrepay4.Text);
                    tPay5 = decimal.Parse(txtTaxPrepay5.Text);
                    tPay6 = decimal.Parse(txtTaxPrepay6.Text);
                    tPay7 = decimal.Parse(txtTaxPrepay7.Text);
                    tPay8 = decimal.Parse(txtTaxPrepay8.Text);

                    taxP = taxP - tPay1 - tPay2 - tPay3 - tPay4 - tPay5 - tPay6 - tPay7 - tPay8; // odejmuje wpłacone zaliczki od podatku

                    txtTaxPrepay9.Text = taxP.ToString("n0");

                    txtMonthRevenue9.Text = Calculate(decimal.Parse(txtGrowRevenue9.Text), decimal.Parse(txtGrowRevenue8.Text)).ToString("n2");
                    txtMonthCost9.Text = Calculate(decimal.Parse(txtInCost9.Text), decimal.Parse(txtInCost9.Text)).ToString("n2");
                    txtMonthIncome9.Text = Calculate(decimal.Parse(txtMonthRevenue9.Text), decimal.Parse(txtMonthCost8.Text)).ToString();

                    ShowTotal();
                }
                //jeżeli usniemy / wyzerujemy przychood i koszty to zeruje nam odpowiednie textboxy
                else if (decimal.Parse(txtGrowRevenue9.Text) == 0 || decimal.Parse(txtGrowRevenue9.Text) == 0)
                {
                    txtMonthRevenue9.Text = "0,00";
                    txtMonthCost9.Text = "0,00";
                    txtSIns9.Text = "0,00";
                    txtHIns9.Text = "0,00";
                    txtMonthIncome9.Text = "0,00";
                    txtAmountDed9.Text = "0,00";
                    txtTaxPrepay9.Text = "0,00";

                    ShowTotal();
                }
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - [wrzesień]";
            }
        }
        //-----------------------------------------------------------------------------------------------------------------Oblicza: 10 rata
        private void TaxPrepayMonth10()
        {
            try
            {
                // taxP - podatek od calosci - trzeba odjąć wszystkie wplacone raty !!!
                // amD .. amD2 - jest to skladka zdrowotna, którą się odlicz od podatku
                // tPay1 ... tPay12 - zsumowane wpłacone raty z poprzednich m-cy, które odejmiemy od taxP (od całości podatku)

                decimal tPay1, tPay2, tPay3, tPay4, tPay5, tPay6, tPay7, tPay8, tPay9;
                decimal taxP, amD, amD1, amD2, amD3, amD4, amD5, amD6, amD7, amD8, amD9, amD10;
                decimal SIns, SIns1, SIns2, SIns3, SIns4, SIns5, SIns6, SIns7, SIns8, SIns9, SIns10;

                //przychód (narastający!) za dany misiąc musi buć równy lub większy od porzedniego misiąca i większy od zera które jest przypisane na starcie
                if (decimal.Parse(txtGrowRevenue10.Text) >= decimal.Parse(txtGrowRevenue9.Text) && decimal.Parse(txtGrowRevenue10.Text) > 0)
                {
                    SIns1 = decimal.Parse(txtSIns1.Text);
                    SIns2 = decimal.Parse(txtSIns2.Text);
                    SIns3 = decimal.Parse(txtSIns3.Text);
                    SIns4 = decimal.Parse(txtSIns4.Text);
                    SIns5 = decimal.Parse(txtSIns5.Text);
                    SIns6 = decimal.Parse(txtSIns6.Text);
                    SIns7 = decimal.Parse(txtSIns7.Text);
                    SIns8 = decimal.Parse(txtSIns8.Text);
                    SIns9 = decimal.Parse(txtSIns9.Text);
                    SIns10 = decimal.Parse(txtSIns10.Text);

                    SIns = SIns1 + SIns2 + SIns3 + SIns4 + SIns5 + SIns6 + SIns7 + SIns8 + SIns9 + SIns10;

                    amD1 = CalculateAmountDed(decimal.Parse(txtHIns1.Text));
                    amD2 = CalculateAmountDed(decimal.Parse(txtHIns2.Text));
                    amD3 = CalculateAmountDed(decimal.Parse(txtHIns3.Text));
                    amD4 = CalculateAmountDed(decimal.Parse(txtHIns4.Text));
                    amD5 = CalculateAmountDed(decimal.Parse(txtHIns5.Text));
                    amD6 = CalculateAmountDed(decimal.Parse(txtHIns6.Text));
                    amD7 = CalculateAmountDed(decimal.Parse(txtHIns7.Text));
                    amD8 = CalculateAmountDed(decimal.Parse(txtHIns8.Text));
                    amD9 = CalculateAmountDed(decimal.Parse(txtHIns9.Text));
                    amD10 = CalculateAmountDed(decimal.Parse(txtHIns10.Text));

                    amD = amD1 + amD2 + amD3 + amD4 + amD5 + amD6 + amD7 + amD8 + amD9 + amD10;

                    taxP = CalculateTaxPrepayment(decimal.Parse(txtGrowRevenue10.Text), decimal.Parse(txtInCost10.Text), SIns, amD); //NARASTAJĄCY PRZYCHÓD !!!!
                    txtAmountDed10.Text = CalculateAmountDed(decimal.Parse(txtHIns10.Text)).ToString("n2");

                    tPay1 = decimal.Parse(txtTaxPrepay1.Text);
                    tPay2 = decimal.Parse(txtTaxPrepay2.Text);
                    tPay3 = decimal.Parse(txtTaxPrepay3.Text);
                    tPay4 = decimal.Parse(txtTaxPrepay4.Text);
                    tPay5 = decimal.Parse(txtTaxPrepay5.Text);
                    tPay6 = decimal.Parse(txtTaxPrepay6.Text);
                    tPay7 = decimal.Parse(txtTaxPrepay7.Text);
                    tPay8 = decimal.Parse(txtTaxPrepay8.Text);
                    tPay9 = decimal.Parse(txtTaxPrepay9.Text);

                    taxP = taxP - tPay1 - tPay2 - tPay3 - tPay4 - tPay5 - tPay6 - tPay7 - tPay8 - tPay9; // odejmuje wpłacone zaliczki od podatku

                    txtTaxPrepay10.Text = taxP.ToString("n0");

                    txtMonthRevenue10.Text = Calculate(decimal.Parse(txtGrowRevenue10.Text), decimal.Parse(txtGrowRevenue9.Text)).ToString("n2");
                    txtMonthIncome10.Text = Calculate(decimal.Parse(txtMonthRevenue10.Text), decimal.Parse(txtMonthCost10.Text)).ToString();
                    txtMonthCost10.Text = Calculate(decimal.Parse(txtInCost10.Text), decimal.Parse(txtInCost9.Text)).ToString("n2");

                    ShowTotal();
                }
                //jeżeli usniemy / wyzerujemy przychood i koszty to zeruje nam odpowiednie textboxy
                else if (decimal.Parse(txtGrowRevenue10.Text) == 0 || decimal.Parse(txtGrowRevenue10.Text) == 0)
                {
                    txtMonthRevenue10.Text = "0,00";
                    txtMonthCost10.Text = "0,00";
                    txtSIns10.Text = "0,00";
                    txtHIns10.Text = "0,00";
                    txtMonthIncome10.Text = "0,00";
                    txtAmountDed10.Text = "0,00";
                    txtTaxPrepay10.Text = "0,00";

                    ShowTotal();
                }
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - [październik]";
            }
        }
        //-----------------------------------------------------------------------------------------------------------------Oblicza: 11 rata
        private void TaxPrepayMonth11()
        {
            try
            {
                // taxP - podatek od calosci - trzeba odjąć wszystkie wplacone raty !!!
                // amD .. amD2 - jest to skladka zdrowotna, którą się odlicz od podatku
                // tPay1 ... tPay12 - zsumowane wpłacone raty z poprzednich m-cy, które odejmiemy od taxP (od całości podatku)

                decimal tPay1, tPay2, tPay3, tPay4, tPay5, tPay6, tPay7, tPay8, tPay9, tPay10;
                decimal taxP, amD, amD1, amD2, amD3, amD4, amD5, amD6, amD7, amD8, amD9, amD10, amD11;
                decimal SIns, SIns1, SIns2, SIns3, SIns4, SIns5, SIns6, SIns7, SIns8, SIns9, SIns10, SIns11;

                //przychód (narastający!) za dany misiąc musi buć równy lub większy od porzedniego misiąca i większy od zera które jest przypisane na starcie
                if (decimal.Parse(txtGrowRevenue11.Text) >= decimal.Parse(txtGrowRevenue10.Text) && decimal.Parse(txtGrowRevenue11.Text) > 0)
                {
                    SIns1 = decimal.Parse(txtSIns1.Text);
                    SIns2 = decimal.Parse(txtSIns2.Text);
                    SIns3 = decimal.Parse(txtSIns3.Text);
                    SIns4 = decimal.Parse(txtSIns4.Text);
                    SIns5 = decimal.Parse(txtSIns5.Text);
                    SIns6 = decimal.Parse(txtSIns6.Text);
                    SIns7 = decimal.Parse(txtSIns7.Text);
                    SIns8 = decimal.Parse(txtSIns8.Text);
                    SIns9 = decimal.Parse(txtSIns9.Text);
                    SIns10 = decimal.Parse(txtSIns10.Text);
                    SIns11 = decimal.Parse(txtSIns11.Text);

                    SIns = SIns1 + SIns2 + SIns3 + SIns4 + SIns5 + SIns6 + SIns7 + SIns8 + SIns9 + SIns10 + SIns11;

                    amD1 = CalculateAmountDed(decimal.Parse(txtHIns1.Text));
                    amD2 = CalculateAmountDed(decimal.Parse(txtHIns2.Text));
                    amD3 = CalculateAmountDed(decimal.Parse(txtHIns3.Text));
                    amD4 = CalculateAmountDed(decimal.Parse(txtHIns4.Text));
                    amD5 = CalculateAmountDed(decimal.Parse(txtHIns5.Text));
                    amD6 = CalculateAmountDed(decimal.Parse(txtHIns6.Text));
                    amD7 = CalculateAmountDed(decimal.Parse(txtHIns7.Text));
                    amD8 = CalculateAmountDed(decimal.Parse(txtHIns8.Text));
                    amD9 = CalculateAmountDed(decimal.Parse(txtHIns9.Text));
                    amD10 = CalculateAmountDed(decimal.Parse(txtHIns10.Text));
                    amD11 = CalculateAmountDed(decimal.Parse(txtHIns11.Text));

                    amD = amD1 + amD2 + amD3 + amD4 + amD5 + amD6 + amD7 + amD8 + amD9 + amD10 + amD11;

                    taxP = CalculateTaxPrepayment(decimal.Parse(txtGrowRevenue11.Text), decimal.Parse(txtInCost11.Text), SIns, amD); //NARASTAJĄCY PRZYCHÓD !!!!
                    txtAmountDed11.Text = CalculateAmountDed(decimal.Parse(txtHIns11.Text)).ToString("n2");

                    tPay1 = decimal.Parse(txtTaxPrepay1.Text);
                    tPay2 = decimal.Parse(txtTaxPrepay2.Text);
                    tPay3 = decimal.Parse(txtTaxPrepay3.Text);
                    tPay4 = decimal.Parse(txtTaxPrepay4.Text);
                    tPay5 = decimal.Parse(txtTaxPrepay5.Text);
                    tPay6 = decimal.Parse(txtTaxPrepay6.Text);
                    tPay7 = decimal.Parse(txtTaxPrepay7.Text);
                    tPay8 = decimal.Parse(txtTaxPrepay8.Text);
                    tPay9 = decimal.Parse(txtTaxPrepay9.Text);
                    tPay10 = decimal.Parse(txtTaxPrepay10.Text);

                    taxP = taxP - tPay1 - tPay2 - tPay3 - tPay4 - tPay5 - tPay6 - tPay7 - tPay8 - tPay9 - tPay10; // odejmuje wpłacone zaliczki od podatku

                    txtTaxPrepay11.Text = taxP.ToString("n0");

                    txtMonthRevenue11.Text = Calculate(decimal.Parse(txtGrowRevenue11.Text), decimal.Parse(txtGrowRevenue10.Text)).ToString("n2");
                    txtMonthIncome11.Text = Calculate(decimal.Parse(txtMonthRevenue11.Text), decimal.Parse(txtMonthCost11.Text)).ToString();
                    txtMonthCost11.Text = Calculate(decimal.Parse(txtInCost11.Text), decimal.Parse(txtInCost10.Text)).ToString("n2");

                    ShowTotal();
                }
                //jeżeli usniemy / wyzerujemy przychood i koszty to zeruje nam odpowiednie textboxy
                else if (decimal.Parse(txtGrowRevenue11.Text) == 0 || decimal.Parse(txtGrowRevenue11.Text) == 0)
                {
                    txtMonthRevenue11.Text = "0,00";
                    txtMonthCost11.Text = "0,00";
                    txtSIns11.Text = "0,00";
                    txtHIns11.Text = "0,00";
                    txtMonthIncome11.Text = "0,00";
                    txtAmountDed11.Text = "0,00";
                    txtTaxPrepay11.Text = "0,00";
                    ShowTotal();
                }
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - [listopad]";
            }
        }
        //-----------------------------------------------------------------------------------------------------------------Oblicza: 12 rata
        private void TaxPrepayMonth12()
        {
            try
            {
                // taxP - podatek od calosci - trzeba odjąć wszystkie wplacone raty !!!
                // amD .. amD2 - jest to skladka zdrowotna, którą się odlicz od podatku
                // tPay1 ... tPay12 - zsumowane wpłacone raty z poprzednich m-cy, które odejmiemy od taxP (od całości podatku)

                decimal tPay1, tPay2, tPay3, tPay4, tPay5, tPay6, tPay7, tPay8, tPay9, tPay10, tPay11;
                decimal taxP, amD, amD1, amD2, amD3, amD4, amD5, amD6, amD7, amD8, amD9, amD10, amD11, amD12;
                decimal SIns, SIns1, SIns2, SIns3, SIns4, SIns5, SIns6, SIns7, SIns8, SIns9, SIns10, SIns11, SIns12;

                //przychód (narastający!) za dany misiąc musi buć równy lub większy od porzedniego misiąca i większy od zera które jest przypisane na starcie
                if (decimal.Parse(txtGrowRevenue12.Text) >= decimal.Parse(txtGrowRevenue11.Text) && decimal.Parse(txtGrowRevenue12.Text) > 0)
                {
                    SIns1 = decimal.Parse(txtSIns1.Text);
                    SIns2 = decimal.Parse(txtSIns2.Text);
                    SIns3 = decimal.Parse(txtSIns3.Text);
                    SIns4 = decimal.Parse(txtSIns4.Text);
                    SIns5 = decimal.Parse(txtSIns5.Text);
                    SIns6 = decimal.Parse(txtSIns6.Text);
                    SIns7 = decimal.Parse(txtSIns7.Text);
                    SIns8 = decimal.Parse(txtSIns8.Text);
                    SIns9 = decimal.Parse(txtSIns9.Text);
                    SIns10 = decimal.Parse(txtSIns10.Text);
                    SIns11 = decimal.Parse(txtSIns11.Text);
                    SIns12 = decimal.Parse(txtSIns12.Text);

                    SIns = SIns1 + SIns2 + SIns3 + SIns4 + SIns5 + SIns6 + SIns7 + SIns8 + SIns9 + SIns10 + SIns11 + SIns12;

                    amD1 = CalculateAmountDed(decimal.Parse(txtHIns1.Text));
                    amD2 = CalculateAmountDed(decimal.Parse(txtHIns2.Text));
                    amD3 = CalculateAmountDed(decimal.Parse(txtHIns3.Text));
                    amD4 = CalculateAmountDed(decimal.Parse(txtHIns4.Text));
                    amD5 = CalculateAmountDed(decimal.Parse(txtHIns5.Text));
                    amD6 = CalculateAmountDed(decimal.Parse(txtHIns6.Text));
                    amD7 = CalculateAmountDed(decimal.Parse(txtHIns7.Text));
                    amD8 = CalculateAmountDed(decimal.Parse(txtHIns8.Text));
                    amD9 = CalculateAmountDed(decimal.Parse(txtHIns9.Text));
                    amD10 = CalculateAmountDed(decimal.Parse(txtHIns10.Text));
                    amD11 = CalculateAmountDed(decimal.Parse(txtHIns11.Text));
                    amD12 = CalculateAmountDed(decimal.Parse(txtHIns12.Text));

                    amD = amD1 + amD2 + amD3 + amD4 + amD5 + amD6 + amD7 + amD8 + amD9 + amD10 + amD11 + amD12;

                    taxP = CalculateTaxPrepayment(decimal.Parse(txtGrowRevenue12.Text), decimal.Parse(txtInCost12.Text), SIns, amD); //NARASTAJĄCY PRZYCHÓD !!!!
                    txtAmountDed12.Text = CalculateAmountDed(decimal.Parse(txtHIns12.Text)).ToString("n2");

                    tPay1 = decimal.Parse(txtTaxPrepay1.Text);
                    tPay2 = decimal.Parse(txtTaxPrepay2.Text);
                    tPay3 = decimal.Parse(txtTaxPrepay3.Text);
                    tPay4 = decimal.Parse(txtTaxPrepay4.Text);
                    tPay5 = decimal.Parse(txtTaxPrepay5.Text);
                    tPay6 = decimal.Parse(txtTaxPrepay6.Text);
                    tPay7 = decimal.Parse(txtTaxPrepay7.Text);
                    tPay8 = decimal.Parse(txtTaxPrepay8.Text);
                    tPay9 = decimal.Parse(txtTaxPrepay9.Text);
                    tPay10 = decimal.Parse(txtTaxPrepay10.Text);
                    tPay11 = decimal.Parse(txtTaxPrepay11.Text);

                    taxP = taxP - tPay1 - tPay2 - tPay3 - tPay4 - tPay5 - tPay6 - tPay7 - tPay8 - tPay9 - tPay10 - tPay11; // odejmuje wpłacone zaliczki od podatku

                    txtTaxPrepay12.Text = taxP.ToString("n0");

                    txtMonthRevenue12.Text = Calculate(decimal.Parse(txtGrowRevenue12.Text), decimal.Parse(txtGrowRevenue11.Text)).ToString("n2");
                    txtMonthIncome12.Text = Calculate(decimal.Parse(txtMonthRevenue12.Text), decimal.Parse(txtMonthCost12.Text)).ToString();
                    txtMonthCost12.Text = Calculate(decimal.Parse(txtInCost12.Text), decimal.Parse(txtInCost11.Text)).ToString("n2");

                    ShowTotal();
                }
                //jeżeli usniemy / wyzerujemy przychood i koszty to zeruje nam odpowiednie textboxy
                else if (decimal.Parse(txtGrowRevenue12.Text) == 0 || decimal.Parse(txtGrowRevenue12.Text) == 0)
                {
                    txtMonthRevenue12.Text = "0,00";
                    txtMonthCost12.Text = "0,00";
                    txtSIns12.Text = "0,00";
                    txtHIns12.Text = "0,00";
                    txtMonthIncome12.Text = "0,00";
                    txtAmountDed12.Text = "0,00";
                    txtTaxPrepay12.Text = "0,00";
                    ShowTotal();
                }

            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - [grudzień]";
            }
        }
        // --------------------------------------------------------------------------------- RATY ---------------------------------------------KONIEC
        //pytaj czy zapisać przed wyjściem
        private void SaveExit()
        {
            DialogResult dialogResult = MessageBox.Show("Czy zapisać dane przed zamknięciem aplikacji !", "Zamykanie aplikacji", MessageBoxButtons.YesNoCancel);
            if (dialogResult == DialogResult.Yes)
            {
                SaveRevDB();
                SaveCostDB();
                SaveSocial();
                SaveHealth();
                this.Close();
            }
            else if (dialogResult == DialogResult.No)
            {
                this.Close();
            }
            else if (dialogResult == DialogResult.Cancel)
            {
                //----
            }
        }

        //pozwala wpusać tylko cyfry w textboxach
        private void OnlyNumbers(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        // ----------------------------------------------------------------------------TextBOXy-------METODY----RAT---------------------------------
        //--------------------------------------------------------------------------------------------------------------------------1
        private void TxtGrowRevenue1_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth1();
        }
        private void TxtInCost1_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth1();
        }
        private void TxtHIns1_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth1();
        }
        private void TxtSIns1_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth1();
        }
        //--------------------------------------------------------------------------------------------------------------------------2
        private void txtGrowRevenue2_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth2();
        }
        private void txtInCost2_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth2();
        }
        private void txtHIns2_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth2();
        }
        private void txtSIns2_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth2();
        }
        //--------------------------------------------------------------------------------------------------------------------------3
        private void txtGrowRevenue3_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth3();
        }
        private void txtInCost3_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth3();
        }
        private void txtHIns3_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth3();
        }
        private void txtSIns3_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth3();
        }
        //--------------------------------------------------------------------------------------------------------------------------7
        private void txtGrowRevenue4_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth4();
        }
        private void txtInCost4_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth4();
        }
        private void txtHIns4_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth4();
        }
        private void txtSIns4_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth4();
        }
        //-------------------------------------------------------------------------------------------------------------------------5
        private void txtGrowRevenue5_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth5();
        }
        private void txtInCost5_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth5();
        }
        private void txtHIns5_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth5();
        }
        private void txtSIns5_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth5();
        }
        //-------------------------------------------------------------------------------------------------------------------------6
        private void txtGrowRevenue6_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth6();
        }
        private void txtInCost6_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth6();
        }
        private void txtHIns6_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth6();
        }
        private void txtSIns6_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth6();
        }
        //-------------------------------------------------------------------------------------------------------------------------7
        private void txtGrowRevenue7_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth7();
        }
        private void txtInCost7_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth7();
        }
        private void txtHIns7_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth7();
        }
        private void txtSIns7_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth7();
        }
        //-------------------------------------------------------------------------------------------------------------------------8
        private void txtGrowRevenue8_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth8();
        }
        private void txtInCost8_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth8();
        }
        private void txtHIns8_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth8();
        }
        private void txtSIns8_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth8();
        }
        //-------------------------------------------------------------------------------------------------------------------------9
        private void txtGrowRevenue9_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth9();
        }
        private void txtInCost9_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth9();
        }
        private void txtHIns9_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth9();
        }
        private void txtSIns9_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth9();
        }
        //------------------------------------------------------------------------------------------------------------------------10
        private void txtGrowRevenue10_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth10();
        }
        private void txtInCost10_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth10();
        }
        private void txtHIns10_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth10();
        }
        private void txtSIns10_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth10();
        }
        //-----------------------------------------------------------------------------------------------------------------------11
        private void txtGrowRevenue11_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth11();
        }
        private void txtInCost11_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth11();
        }
        private void txtHIns11_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth11();
        }
        private void txtSIns11_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth11();
        }
        //-----------------------------------------------------------------------------------------------------------------------12
        private void TxtGrowRevenue12_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth12();
        }
        private void txtInCost12_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth12();
        }
        private void txtHIns12_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth12();
        }
        private void txtSIns12_TextChanged(object sender, EventArgs e)
        {
            TaxPrepayMonth12();
        }
        // ----------------------------------------------------------------------------TextBOXy--------------------------------KONIEC
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            timer1.Stop();
            lblInfo.Text = "";
        }
        // ----------------------------------------------------------------------------Przyciski--------------------------------

        //----------------------------------------------------Textboxy-----------------------TYLKO LICZBY-----------------------------------------

        //- Pyta przy zamknoęciu czy zapisać
        private void zakmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveExit();
        }

        private void sprawdźStwórzBazToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckDb();
        }

        private void wczytajDaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadAllFromDB();
            lblInfo.Text = "... Wczytano dane z bazy ...";
        }

        private void zapiszDaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveRevDB();
            SaveCostDB();
            SaveSocial();
            SaveHealth();
            timer1.Start();
            lblInfo.Text = "... Zapisano dane do bazy ...";
        }

        //przeciąganie okna
        private void plSql_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }
        //przeciąganie okna
        private void plSql_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }
        //przeciąganie okna
        private void plSql_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        //data i godzina
        private void timer2_Tick(object sender, EventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            this.lblDateTime.Text = dateTime.ToString("dd MMM yyyy - HH:mm:ss", CultureInfo.CreateSpecificCulture("pl-PL"));
        }

        //przyciski : minimalizuj, max, zamknij
        private void llblClose_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveExit();
        }
        private void llblMax_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {

                llblMax.Text = "Maksymalizuj";
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                llblMax.Text = "  Przywróć  ";
            }
        }
        private void llblMin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        //przenosi wpłaty Zus na nast. m-c.
        private void copyZus1_Click(object sender, EventArgs e)
        {
            txtSIns2.Text = txtSIns1.Text;
            txtHIns2.Text = txtHIns1.Text;
        }

        private void copyZus2_Click(object sender, EventArgs e)
        {
            txtSIns3.Text = txtSIns2.Text;
            txtHIns3.Text = txtHIns2.Text;
        }

        private void copyZus3_Click(object sender, EventArgs e)
        {
            txtSIns4.Text = txtSIns3.Text;
            txtHIns4.Text = txtHIns3.Text;
        }

        private void copyZus4_Click(object sender, EventArgs e)
        {
            txtSIns5.Text = txtSIns4.Text;
            txtHIns5.Text = txtHIns4.Text;
        }

        private void copyZus5_Click(object sender, EventArgs e)
        {
            txtSIns6.Text = txtSIns5.Text;
            txtHIns6.Text = txtHIns5.Text;
        }

        private void copyZus6_Click(object sender, EventArgs e)
        {
            txtSIns7.Text = txtSIns6.Text;
            txtHIns7.Text = txtHIns6.Text;
        }

        private void copyZus7_Click(object sender, EventArgs e)
        {
            txtSIns8.Text = txtSIns7.Text;
            txtHIns8.Text = txtHIns7.Text;
        }

        private void copyZus8_Click(object sender, EventArgs e)
        {
            txtSIns9.Text = txtSIns8.Text;
            txtHIns9.Text = txtHIns8.Text;
        }

        private void copyZus9_Click(object sender, EventArgs e)
        {
            txtSIns10.Text = txtSIns9.Text;
            txtHIns10.Text = txtHIns9.Text;
        }

        private void copyZus10_Click(object sender, EventArgs e)
        {
            txtSIns11.Text = txtSIns10.Text;
            txtHIns11.Text = txtHIns10.Text;
        }

        private void copyZus11_Click(object sender, EventArgs e)
        {
            txtSIns12.Text = txtSIns11.Text;
            txtHIns12.Text = txtHIns11.Text;
        }

        private void copyTax1_Click(object sender, EventArgs e)
        {
            txtPaid1.Text = txtTaxPrepay1.Text;
        }

        private void copyTax2_Click(object sender, EventArgs e)
        {
            txtPaid2.Text = txtTaxPrepay2.Text;
        }

        private void copyTax3_Click(object sender, EventArgs e)
        {
            txtPaid3.Text = txtTaxPrepay3.Text;
        }

        private void copyTax4_Click(object sender, EventArgs e)
        {
            txtPaid4.Text = txtTaxPrepay4.Text;
        }

        private void copyTax5_Click(object sender, EventArgs e)
        {
            txtPaid5.Text = txtTaxPrepay5.Text;
        }

        private void copyTax6_Click(object sender, EventArgs e)
        {
            txtPaid6.Text = txtTaxPrepay6.Text;
        }

        private void copyTax7_Click(object sender, EventArgs e)
        {
            txtPaid7.Text = txtTaxPrepay7.Text;
        }

        private void copyTax8_Click(object sender, EventArgs e)
        {
            txtPaid8.Text = txtTaxPrepay8.Text;
        }

        private void copyTax9_Click(object sender, EventArgs e)
        {
            txtPaid9.Text = txtTaxPrepay9.Text;
        }

        private void copyTax10_Click(object sender, EventArgs e)
        {
            txtPaid10.Text = txtTaxPrepay10.Text;
        }

        private void copyTax11_Click(object sender, EventArgs e)
        {
            txtPaid11.Text = txtTaxPrepay11.Text;
        }

        private void copyTax12_Click(object sender, EventArgs e)
        {
            txtPaid12.Text = txtTaxPrepay12.Text;
        }

        private void txtPaid1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal w;
                w = CheckPayment(decimal.Parse(txtTaxPrepay1.Text), decimal.Parse(txtPaid1.Text));

                if (w < 0)
                {
                    txtPlusMinus1.ForeColor = Color.Red;
                }
                else
                {
                    txtPlusMinus1.ForeColor = Color.Green;
                }
                txtPlusMinus1.Text = w.ToString();
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - wpłaty - [styczeń].";
            }
        }

        private void txtPaid2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal w;
                w = CheckPayment(decimal.Parse(txtTaxPrepay2.Text), decimal.Parse(txtPaid2.Text));

                if (w < 0)
                {
                    txtPlusMinus2.ForeColor = Color.Red;
                }
                else
                {
                    txtPlusMinus2.ForeColor = Color.Green;
                }
                txtPlusMinus2.Text = w.ToString();
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - wpłaty - [luty].";
            }
        }

        private void txtPaid3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal w;
                w = CheckPayment(decimal.Parse(txtTaxPrepay3.Text), decimal.Parse(txtPaid3.Text));

                if (w<0)
                {
                    txtPlusMinus3.ForeColor = Color.Red;
                }
                else
                {
                    txtPlusMinus3.ForeColor = Color.Green;
                }
                txtPlusMinus3.Text = w.ToString();
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - wpłaty - [marzec].";
            }

        }

        private void txtPaid4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal w;
                w = CheckPayment(decimal.Parse(txtTaxPrepay4.Text), decimal.Parse(txtPaid4.Text));

                if (w < 0)
                {
                    txtPlusMinus4.ForeColor = Color.Red;
                }
                else
                {
                    txtPlusMinus4.ForeColor = Color.Green;
                }
                txtPlusMinus4.Text = w.ToString();
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - wpłaty - [kwiecień].";
            }
        }

        private void txtPaid5_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal w;
                w = CheckPayment(decimal.Parse(txtTaxPrepay5.Text), decimal.Parse(txtPaid5.Text));

                if (w < 0)
                {
                    txtPlusMinus5.ForeColor = Color.Red;
                }
                else
                {
                    txtPlusMinus5.ForeColor = Color.Green;
                }
                txtPlusMinus5.Text = w.ToString();
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - wpłaty - [kwiecień].";
            }
        }

        private void txtPaid6_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal w;
                w = CheckPayment(decimal.Parse(txtTaxPrepay6.Text), decimal.Parse(txtPaid6.Text));

                if (w < 0)
                {
                    txtPlusMinus6.ForeColor = Color.Red;
                }
                else
                {
                    txtPlusMinus6.ForeColor = Color.Green;
                }
                txtPlusMinus6.Text = w.ToString();
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - wpłaty - [maj].";
            }
        }

        private void txtPaid7_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal w;
                w = CheckPayment(decimal.Parse(txtTaxPrepay7.Text), decimal.Parse(txtPaid7.Text));

                if (w < 0)
                {
                    txtPlusMinus7.ForeColor = Color.Red;
                }
                else
                {
                    txtPlusMinus7.ForeColor = Color.Green;
                }
                txtPlusMinus7.Text = w.ToString();
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - wpłaty - [kwiecień].";
            }
        }

        private void txtPaid8_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal w;
                w = CheckPayment(decimal.Parse(txtTaxPrepay8.Text), decimal.Parse(txtPaid8.Text));

                if (w < 0)
                {
                    txtPlusMinus8.ForeColor = Color.Red;
                }
                else
                {
                    txtPlusMinus8.ForeColor = Color.Green;
                }
                txtPlusMinus8.Text = w.ToString();
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - wpłaty - [kwiecień].";
            }
        }

        private void txtPaid9_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal w;
                w = CheckPayment(decimal.Parse(txtTaxPrepay9.Text), decimal.Parse(txtPaid9.Text));

                if (w < 0)
                {
                    txtPlusMinus9.ForeColor = Color.Red;
                }
                else
                {
                    txtPlusMinus9.ForeColor = Color.Green;
                }
                txtPlusMinus9.Text = w.ToString();
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - wpłaty - [kwiecień].";
            }
        }

        private void txtPaid10_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal w;
                w = CheckPayment(decimal.Parse(txtTaxPrepay10.Text), decimal.Parse(txtPaid10.Text));

                if (w < 0)
                {
                    txtPlusMinus10.ForeColor = Color.Red;
                }
                else
                {
                    txtPlusMinus10.ForeColor = Color.Green;
                }
                txtPlusMinus10.Text = w.ToString();
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - wpłaty - [kwiecień].";
            }
        }

        private void txtPaid11_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal w;
                w = CheckPayment(decimal.Parse(txtTaxPrepay11.Text), decimal.Parse(txtPaid11.Text));

                if (w < 0)
                {
                    txtPlusMinus11.ForeColor = Color.Red;
                }
                else
                {
                    txtPlusMinus11.ForeColor = Color.Green;
                }
                txtPlusMinus11.Text = w.ToString();
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - wpłaty - [kwiecień].";
            }
        }

        private void txtPaid12_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal w;
                w = CheckPayment(decimal.Parse(txtTaxPrepay12.Text), decimal.Parse(txtPaid12.Text));

                if (w < 0)
                {
                    txtPlusMinus12.ForeColor = Color.Red;
                }
                else
                {
                    txtPlusMinus12.ForeColor = Color.Green;
                }
                txtPlusMinus12.Text = w.ToString();
            }
            catch (Exception ex)
            {
                timer1.Start();
                lblInfo.Text = ex.Message.ToString() + " - wpłaty - [kwiecień].";
            }
        }
    }
}

