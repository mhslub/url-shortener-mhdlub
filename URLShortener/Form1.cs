/*
 * The major concepts used in this URL shortener:
 * 1)All URLs are stored in a repository on the machine hardDrive.
 * 2)Each URL is assigned a unique auto increased number in the repository.
 * 3)Either base62 or base36 can be used for encoding and decoding numbers into unique short representations in 62 or 36 spaces respectively.
 * 4)Base62 has longer alphabet ==> gives shorter representations.
 * 5)If a URL is provided, the program checks the repository, if found then extract its associated number and Encode it. Otherwise add new entry to the repository.
 * 6)Dictionary is used to do insertion and lookup operations on the repository.
 * 7)When closing the session, the new added entries are copied from the dictionary to the repository file in the hardDrive.
 * 
 * */

using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace URLShortener
{
    public partial class Form1 : Form
    {

        private Dictionary<string, long> all_urls;//this dictionary holds the contents of the repository
        private BaseXX basexx; // to be used for encoding
        //the following will help determine the new entries added in the current session so we can update the database after current session ends
        private long last_id, last_session_id;//the id of the last saved url in the currecnt session plus the previous session last id


        public Form1()
        {
            InitializeComponent();
            all_urls = new Dictionary<string, long>();
            last_id = readRepository();
            last_session_id = last_id;
            comboBox1.SelectedItem = comboBox1.Items[1];//the default is base62 
            comboBox2.SelectedItem = comboBox2.Items[1];//the default is base62
            label6.Visible = true;
            //the default base is base62
            basexx = new BaseXX("0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
            //--------------------------------------------------------------------
      
           
        }

        //the following reads the repository and returns the id of last saved url
        private long readRepository()
        {
            //read the repository contents and save in the dictionary
            StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + "\\url_repository.mlub");
            string line = sr.ReadLine();
            long last_id = 0;
            while (line != null)
            {
                string[] split = { " " };//split the line on the space
                string[] res = line.Split(split, StringSplitOptions.RemoveEmptyEntries);
                //res[0] is the id and res[1] is the url
                last_id=(long)Double.Parse(res[0]);
                all_urls.Add(res[1], last_id);
                //-----------------------------------------------------
                line = sr.ReadLine();

            }
            sr.Close();
            return last_id;
        }

        //the following checks url validity
        private bool urlValidation(string url)
        {
            bool ok = false;

            string[] splits = { "/" };
            string[] res = textBox1.Text.Split(splits, StringSplitOptions.RemoveEmptyEntries);

            //the following is to check the existance of http:// or https://
            if ((res[0].Contains("http:") && url.Contains("http://")) || (res[0].Contains("https:") && url.Contains("https://")))
            {
                //asimple regular expression validation of the input url
                if (Regex.IsMatch(res[1].ToLower(), @"^(www.)?[a-z-0-9]+(\.[a-zA-Z]{2,6})+$"))
                {
                    ok = true;
                }
            }
            //if not contains http:// or https://
            else
            {
                if (Regex.IsMatch(res[0].ToLower(), @"^(www.)?[a-z-0-9]+(\.[a-zA-Z]{2,6})+$"))
                {
                    ok = true;
                }
            }

            return ok;
        }
        //shortening process
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals(""))
            {
                MessageBox.Show("Enter a URL to shorten");
                return;
            }
            
            //first check if correct url is provided
            if (urlValidation(textBox1.Text))
            {
                //chech if it is already in the database
                if (all_urls.ContainsKey(textBox1.Text))
                {
                    long id = all_urls[textBox1.Text];
                    textBox3.Text = basexx.Encode(id);
                }

                else
                {
                    long id = ++last_id; //get the id
                    //add the new url to the dictionary
                    all_urls.Add(textBox1.Text, id);
                    textBox3.Text = basexx.Encode(id);

                }
            }
            else
            {
                MessageBox.Show("Wrong URL!", "Error");
                textBox3.Text = "";
            }
            
            
        }

        //on session close, update repository
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //when closing the session, we need to update our database with new entries
            if (last_id != last_session_id)
            {
                //write on the same file without deleting its contents
                StreamWriter sw = new StreamWriter(System.Environment.CurrentDirectory + "\\url_repository.mlub", true);
                for (long i = last_session_id + 1; i <= last_id ; i++)
                {
                    foreach (string key in all_urls.Keys)
                    {
                        if (all_urls[key] == i)
                        {
                            sw.WriteLine(i.ToString() + " " + key);
                            break;//because ids are unique too
                        }
                    }
                }

                sw.Close();
            }
        }

        //redirect a shortened url
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox4.Text.Equals(""))
            {
                MessageBox.Show("Enter a shortened URL to go!");
                return;
            }

            //apply decoding
            long id = basexx.Decode(textBox4.Text);
            //if the dictionary contains such id
            if (all_urls.ContainsValue(id))
            {
                //lookup for the corresponding url
                foreach (string key in all_urls.Keys)
                {
                    if (all_urls[key] == id)
                    {
                        //redirect the user to the corresponding url
                        if (key.Contains("http"))
                            System.Diagnostics.Process.Start(key);
                        else
                            System.Diagnostics.Process.Start("http://"+key);
                        break;//stop looking
                    }
                }
            }
            else
            {
                MessageBox.Show("Your entry does not exist is the database.\nMake sure to use a correct pre-shortened input.", "Error");
            }
        }

        //control procedures======================================================
        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            //change the base as selected index 0 for base36 and 1 for base62
            basexx = (comboBox1.SelectedIndex == 0) ? new BaseXX("0123456789abcdefghijklmnopqrstuvwxyz") : new BaseXX("0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
            label6.Visible = (comboBox1.SelectedIndex == 0) ? false : true;
       
        }

        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            //change the base as selected index 0 for base36 and 1 for base62
            basexx = (comboBox2.SelectedIndex == 0) ? new BaseXX("0123456789abcdefghijklmnopqrstuvwxyz") : new BaseXX("0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
        
        }
    }
}
