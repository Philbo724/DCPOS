using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.XPath;

namespace DiscConnectionSuite.Purchase
{
    /// <summary>
    /// Interaction logic for Purchase.xaml
    /// </summary>
    public partial class Purchase : Window
    {
        private string userId, bpd;
        public Purchase(string newUserId, string newBpd)
        {
            InitializeComponent();
            userId = newUserId;
            bpd = newBpd;
            txBarcode.Focus();
        }

        void txBarcode_TextChanged(Object obj, EventArgs e)
        {
            if (!txBarcode.Text.ToString().Equals(string.Empty))
            {
                CallAmazon();
            }
        }

        private void CallAmazon() 
        {
            try {

                //TEST UPC: 724352932542
                //AWSAccessKeyId: AKIAJYECRRRIAHLHVSLQ
                //AssociateTag: disc0e4-20
                //Secret Key: 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

                string barcode = txBarcode.Text;
                bool rankOkay = true;
                lbSalesRank.Foreground = Brushes.Red;

                //Do first request (getting Title, Image, ASIN, and Sales Rank)
                string requestUrl = "http://free.apisigning.com/onca/xml?Service=AWSECommerceService&Operation=ItemSearch&AWSAccessKeyId=AKIAJYECRRRIAHLHVSLQ" +
                "&AssociateTag=disc0e4-20&Version=2011-08-01&SearchIndex=All&Condition=Used&Keywords=" + barcode + "&ResponseGroup=ItemAttributes,Images,SalesRank";

                WebRequest request = HttpWebRequest.Create(requestUrl);
                request.Method = "GET";
                WebResponse response = request.GetResponse();

                //Parse the contents from the response to a stream object
                System.IO.Stream stream = response.GetResponseStream();

                //Create a reader for the stream object
                XmlTextReader reader = new XmlTextReader(stream);
                
                //Read from the stream object using the reader, put the contents in a string
                XmlDocument doc = new XmlDocument();
                XmlNodeList asin;
                XmlNodeList desc;
                XmlNodeList rank;
                XmlNodeList image;
                
                while(reader.Read()) {
                    doc.Load(reader);

                    //Getting nodes for ASIN, image, sales rank, and description
                    desc = doc.GetElementsByTagName("Title");
                    asin = doc.GetElementsByTagName("ASIN");

                    if (CheckASIN(asin[0].InnerText) == true)
                    {
                        ErrMsg.Content = "This item has been blacklisted. DO NOT PURCHASE!";
                        return;
                    }

                    try {
                        rank = doc.GetElementsByTagName("SalesRank");
                        lbSalesRank.Content = rank[0].InnerText;

                        if(Convert.ToInt32(rank[0].InnerText) > 100000) {
                            lbSalesRank.Foreground = Brushes.Red;
                            rankOkay = false;
                        }
                    } catch (Exception) {
                        lbSalesRank.Content = "0";
                        lbSalesRank.Foreground = Brushes.Red;
                        ErrMsg.Content = "No sales rank available. Check Amazon.com for more information.";
                    }
                
                    image = doc.GetElementsByTagName("LargeImage");

                    foreach(XmlNode I_Node in image) {
                        Uri imageUri = new Uri(I_Node["URL"].InnerText, UriKind.Absolute);
                        BitmapImage imageBitmap = new BitmapImage(imageUri);
                        imImage.Source = imageBitmap;
                        break;
                    }
                    
                    txTitle.Text = desc[0].InnerText;
                    txBarcode.Text = asin[0].InnerText;
                }

                //string searchUrl = "http://www.amazon.com/gp/offer-listing/";
                txURL.Text = "www.amazon.com/gp/offer-listing/" + txBarcode.Text.ToString();

                //Formatting Sales Rank to have commas for thousands, etc.
                lbSalesRank.Content = System.String.Format("{0:n0}", Convert.ToInt64(lbSalesRank.Content.ToString()));

                //Using ASIN just in case something's goofy
                barcode = txBarcode.Text;

                //Building new request for all offers (getting lowest used price, Offers Url)
                requestUrl = "http://free.apisigning.com/onca/xml?Service=AWSECommerceService&Operation=ItemSearch&AWSAccessKeyId=AKIAJYECRRRIAHLHVSLQ" +
                "&AssociateTag=disc0e4-20&Version=2011-08-01&SearchIndex=All&Condition=Used&Keywords=" + barcode + "&ResponseGroup=OfferSummary";

                request = HttpWebRequest.Create(requestUrl);
                request.Method = "GET";
                response = request.GetResponse();

                //Parse the contents from the response to a stream object
                stream = response.GetResponseStream();
                
                //Create a reader for the stream object
                reader = new XmlTextReader(stream);
                
                //Read from the stream object using the reader, put the contents in a string
                doc = new XmlDocument();
                XmlNodeList offersUrl, usedPrices, newPrices;
                string lowest = string.Empty;
                string nLowest = string.Empty;

                while(reader.Read()) {
                    doc.Load(reader);
                    
                    //Getting nodes for lowest used price, Offers Url
                    offersUrl= doc.GetElementsByTagName("MoreSearchResultsUrl");

                    try {
                        usedPrices = doc.GetElementsByTagName("LowestUsedPrice");
                        
                        foreach(XmlNode P_Node in usedPrices) {
                            lowest = P_Node["FormattedPrice"].InnerText;
                            break;
                        }
                    } catch(Exception) {
                        lowest = "$.00";
                    }

                    try {
                        newPrices = doc.GetElementsByTagName("LowestNewPrice");

                        foreach(XmlNode N_Node in newPrices) {
                            nLowest = N_Node["FormattedPrice"].InnerText;
                            break;
                        }

                    } catch(Exception) {
                        nLowest = "$.00";
                    }
                }


                if(lowest.Equals(string.Empty)) {
                    ErrMsg.Content = "No used prices are available on Amazon.com at this time. Click \"See Offers\" button.";
                    txDCPrice.Focus();
                    return;
                }
            
                if(!nLowest.Equals(string.Empty)) {
                    txAmazonNew.Text = nLowest;
                } else {
                    nLowest = "$.00";
                    txAmazonNew.Text = nLowest;
                }

                btSearch.IsEnabled = true;
                txAmazonPrice.Text = lowest;

                nLowest = nLowest.Substring(1);
                lowest = lowest.Substring(1);
                
                //Calculating DC Price if Sales Rank is over 100k
                if(!rankOkay) {
                    txDCPrice.Text = ".25";
                    txDCPrice.Focus();
                    return;
                }
                
                double price = Convert.ToDouble(lowest);
                double nPrice = Convert.ToDouble(nLowest);

                if(nPrice < price) {
                    ErrMsg.Content = "New price is lower than used price. Please review on Amazon.com";
                    txDCPrice.Focus();
                    return;
                }
                
                if(price <= 2) {
                    txDCPrice.Text = ".25";
                    btAdd.Focus();
                    return;
                }

                if(price <= 40) {
                    price = price / 2;
                    string strPrice = Convert.ToString(price).Remove(Convert.ToString(price).IndexOf('.'));
                    txDCPrice.Text = strPrice + ".00";
                    btAdd.Focus();
                    return;
                }

                if(price > 40) {
                    ErrMsg.Content = "Item is priced over $40. Please review on Amazon.com.";
                    txDCPrice.Focus();
                }

            } catch(Exception) {
                ErrMsg.Content = "An error occurred while contacting Amazon. Check your connection OR enter the data manually.";
            }
        }

        private bool CheckASIN(string asin)
        {
            //verify that ASIN hasn't been Blacklisted
            try
            {
                DataTable dt = BusLayer.blPurchase.CheckItemBlacklist(asin);
                return (dt.Rows.Count >= 1);
            } catch (Exception) {
                return false;
            }
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            try {
                if(txDCPrice.Text.Equals(string.Empty)) {
                    ErrMsg.Content = "Please input a price for this item.";
                    txDCPrice.Focus();
                    return;
                }
                int i = 0;
                
                //If barcode box is empty, compare descriptions; otherwise, compare BC/ASIN
                if(txBarcode.Text.Equals(string.Empty)) {
                    for(i = 0; i < ItemList.Items.Count; i++) {
                        if(ItemList.Items[i].ToString().Contains(txTitle.Text.ToString())) {
                            if(chAllowDuplicate.IsChecked == false) {
                                ErrMsg.Content = "Please check box to accept duplicate entry. Otherwise, change previous item quantity.";
                                return;
                            }
                        }
                    }
                } else {
                     for(i = 0; i < ItemList.Items.Count; i++) {
                        if(ItemList.Items[i].ToString().Contains(txBarcode.Text.ToString())) {
                            if(chAllowDuplicate.IsChecked == false) {
                                ErrMsg.Content = "Please check box to accept duplicate entry. Otherwise, change previous item quantity.";
                                return;
                            }
                        }
                    }
                }

                double dcPrice = Convert.ToDouble(txDCPrice.Text.ToString());
                if(txQuantity.Text.Equals(string.Empty)) {
                    txQuantity.Text = "1";
                }
                
                int itemQty = Convert.ToInt32(txQuantity.Text.ToString());
                string title = txTitle.Text.ToString();
                
                //Calculating total price of all items (default: 1)
                dcPrice = dcPrice * itemQty;
                
                //Taking "$" off of the new price
                string newPrice = dcPrice.ToString();
                
                //If there is no Total price currently, set total price to 0
                if(lbTotalPrice.Content.ToString().Equals(string.Empty)) {
                    lbTotalPrice.Content = "0";
                }

                double totalPrice = Convert.ToDouble(lbTotalPrice.Content.ToString()) + Convert.ToDouble(newPrice);
                lbTotalPrice.Content = totalPrice.ToString();

                //Adding item to listbox
                ItemList.Items.Add("Price: " + dcPrice + "      " +
                    "Title: " + title + " (" + itemQty + ")" + "      " +
                    "SN: " + txSerial.Text.ToString() + "      " +
                    "BC: " + txBarcode.Text.ToString());


                ClearBoxes();
                
                txBarcode.Focus();

                ItemList.SelectedIndex = ItemList.Items.Count - 1;
            
            } catch(Exception) {
                ErrMsg.Content = "An error occurred. Please try again.";
            }
        }

        private void ClearBoxes()
        {
            txBarcode.Text = string.Empty;
            txTitle.Text = string.Empty;
            txDCPrice.Text = string.Empty;
            txAmazonNew.Text = string.Empty;
            txAmazonPrice.Text = string.Empty;
            imImage.Source = null;
            lbSalesRank.Content = string.Empty;
            lbSalesRank.Foreground = Brushes.Black;
            btSearch.IsEnabled = false;
            txQuantity.Text = "1";
            chAllowDuplicate.IsChecked = false;
        }

        private void btRemove_Click(object sender, RoutedEventArgs e)
        {
            try {
                int selected = ItemList.SelectedIndex;
                
                if(selected < ItemList.Items.Count) {
                    //Subtract price from total
                    string price = ItemList.SelectedItem.ToString();
                    string total = lbTotalPrice.Content.ToString();

                    price = price.Substring(price.IndexOf("Price:") + 6);
                    price = price.Remove(price.IndexOf("Title:")).Trim();
                    
                    double priceNum = Convert.ToDouble(price);
                    double totalNum = Convert.ToDouble(total);

                    totalNum = totalNum - priceNum;

                    lbTotalPrice.Content = totalNum.ToString();
                    
                    //Remove item from list
                    ItemList.Items.Remove(ItemList.Items[selected]);
                }

            } catch(Exception) {
                ErrMsg.Content = "An error occurred. Please try again.";
            }
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            if(chConfirmCancel.IsChecked == false) {
                ErrMsg.Content = "Please check the check box to cancel this transaction.";
                return;
            }
            
            try {
                MainMenu menu = new MainMenu(userId);
                menu.Show();
                this.Close();
            } catch(Exception) {

            }
        }

        private void btFinish_Click(object sender, RoutedEventArgs e)
        {
            try {
                int i;
                
                //Going row by row and parsing data out of the items list
                for(i = ItemList.Items.Count - 1; i >= 0; i--) {
                    string row = ItemList.Items[i].ToString();
                    
                    string upc = row.Substring(row.IndexOf("BC:") + 3);
                    row = row.Remove(row.IndexOf("BC:"));

                    string serial = row.Substring(row.IndexOf("SN:") + 3).Trim();
                    row = row.Remove(row.IndexOf("SN:"));

                    string title = row.Substring(row.IndexOf("Title:") + 6).Trim();
                    row = row.Remove(row.IndexOf("Title:"));

                    string price = row.Substring(row.IndexOf("Price:") + 6).Trim();
                    
                    //Send data to DB
                    if(BusLayer.blPurchase.AddItems(upc, title, serial, price, bpd)) {
                        ItemList.Items.Remove(ItemList.Items[i]);
                    } else {
                        throw new Exception("oops");
                    }
                }
            
                if(ItemList.Items.Count > 0) {
                    ErrMsg.Content = "An error occurred while adding items to DB. Please check your connection and try again.";
                    return;
                }
                AddToExcel();
                Report report = new Report(bpd, userId, lbTotalPrice.Content.ToString().Trim());
                report.Show();
                this.Close();

            } catch(Exception) {
                ErrMsg.Content = "An error occurred. Please verify data and try again.";
            }
        }

        private void AddToExcel()
        {
            string totPrice = lbTotalPrice.Content.ToString();
            string currDate = DateTime.Now.ToShortDateString();
            string filename = currDate + "_registerData.csv";

            if (!File.Exists(filename))
            {
                string clientHeader = "BPD,Date,Total";
                File.WriteAllText(filename, clientHeader);
            }

            string data = bpd + "," + currDate + "," + totPrice;
            File.AppendAllText(filename, data);
        }

        private void btSearch_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(txURL.Text.ToString());
        }

        private void txBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txTitle.Focus();
            }
        }
    }
}
