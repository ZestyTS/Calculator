﻿using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class FrmCalc : Form
    {
        public FrmCalc()
        {
            InitializeComponent();
        }

        //instead of having numerous different buttons, I'm just passing all of them that are a character of text through this
        private void Buttonclick(object sender, EventArgs e)
        {
            //initilizing text string variable
            var text ="";

            //since the sender object could be a Form because of keypresses or a Button by the user clicking the button I have to check
            if (sender is Button)
            {
                //we make a Button variable since we know the sender object is of the class Button
                var btn = (Button)sender;
                //if the length is greater than one, that means it's the exponent which we will show with the carrot symbol
                text = btn.Text.Length > 1 ? @"^" : btn.Text;
            }
            else if (sender is Form)
            {
                //Since it's the form that means that the user is entering things via their keyboard
                var keyevent = (KeyPressEventArgs) e;

                //since keyboard inputs come in ASCII way, we need to convert them to their non ASCII values
                text = keyevent.KeyChar.ToString();

                // e/E is used for equals, so we need to swap them to the correct thing
                text = text.Replace("e", "=").Replace("E", "=");
            }

            //if label1.text is blank then no mathmatical operations should be passed
            if (string.IsNullOrEmpty(label1.Text) && (!char.IsDigit(Convert.ToChar(text)) && (text != @"." && text != @"(" && text != @")")))
                //returns just break out of the function
                return;

            //can't have back to back mathmatical operations
            if ((!char.IsDigit(Convert.ToChar(text)) && (text != @"." && text != @"(" && text != @")")) && label1.Text[label1.Text.Length - 1] == ' ')
                return;

            //if there's an equal sign then that means we already solved the equation so we should start from scratch
            if (label1.Text.Contains("="))
            {
                //I already have a clear button, might as well use the function so I'm not making duplicate code
                clear_Click(sender, e);
            }
            

            //if the button that's being selected is a digit then don't put spaces around it
            label1.Text += char.IsDigit(Convert.ToChar(text)) ? text : " " + text + " ";

            //with my all in one text writer, the periods get spaces around them, so we remove them
            label1.Text = label1.Text.Replace(" . ", ".");
            label1.Text = label1.Text.Replace(" ( ", "(").Replace(" ) ", ")");

            //I all this does is add * if there isn't a mathmatical expression between a digit and ( or )
            if (label1.Text.Length > 1)
            {
                if (char.IsDigit(label1.Text[label1.Text.Length - 2]) && label1.Text[label1.Text.Length - 1] == '(')
                    label1.Text = label1.Text.Substring(0, label1.Text.Length - 1) + @" * (";
                else if (char.IsDigit(label1.Text[label1.Text.Length - 1]) && label1.Text[label1.Text.Length - 2] == ')')
                    label1.Text = label1.Text.Substring(0, label1.Text.Length - 2) + @") * " + text;
            }

            //if there isn't an equal sign then just break out of the function
            if (!label1.Text.Contains("=")) return;

            //counts to make sure there are as many ( as there are )
            var left = label1.Text.Count(x => x == '(');
            var right = label1.Text.Count(x => x == ')');

            //if they aren't the same then someone messed up and the only way to fix it is to just remove everything
            if (left != right)
            {
                clear_Click(sender, e);
                return;
            }

            //to use my evaluate function, there can't be an equal sign
            label1.Text = label1.Text.Replace("= ", "");


            //^ isn't a real mathmatical operation so I can't pass it through my evaluate function
            while (label1.Text.Contains("^"))
            {
                //since I know there is a ^ in my string, I need to find the index of it in the string
                var index = label1.Text.IndexOf("^");

               //I break up the string to land on the last digit of the first exponent
                var temp = label1.Text.Substring(0, index-1);
                var c = temp.ToCharArray();
                //convert to character array so I can reverse it
                Array.Reverse(c);

                var backwards = new string(c);
                //I look through the string backwards to find the space before the string of numbers
                var num = backwards.Contains(" ") ? backwards.IndexOf(" ") : backwards.Length;

                //I remove everything that isn't the numbers of the exponent
                backwards = backwards.Substring(0, num);
                c = backwards.ToCharArray();
                Array.Reverse(c);

                //rereverse it so it's fowards and we set it to a new string to convert to int to get our first exponent number
                var first = Convert.ToInt32(new string(c));


                temp = label1.Text.Substring(index);
                num = temp.Contains(" ") ? temp.IndexOf(" ") : temp.Length;


                //we move the "^ " and everything after the number to get our second number
                var second = Convert.ToInt32(temp.Substring(2, num));


                //Math.Pow is how you do exponent calculations
                var equals = Convert.ToString(Math.Pow(first, second));

                //Now I need to edit my string to add in the change so the Evaluate function can understand it
                label1.Text = label1.Text.Replace(first + " ^ " + second, equals);
            }

            label1.Text = Evaluate(label1.Text);
        }

        //evaluates the mathmatical expression
        private static string Evaluate(string expression)
        {
            var table = new DataTable();

            //adds a column with the word "expression" and puts the first cell as the string expression
            table.Columns.Add("expression", typeof(string), expression);

            var row = table.NewRow();

            //create a new row because this row will have what the expression's answer will be
            table.Rows.Add(row);

            //returns the row that would have the mathmatical expression evaluated
            return (string) row["expression"];
        }

        private void clear_Click(object sender, EventArgs e)
        {
            //Since whatever is in label1.Text gets evaluated, to make it blank, we just set it to ""
            label1.Text = "";
        }

        private void sqrt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(label1.Text)) return;
            var text = label1.Text;

            //this grabs the last character of text
            var lastdigit = text.Substring(text.Length - 1, 1);

            //if the last character isn't a digit then we blank everything out because the user tried to pass in something that wasn't possible
            if (!char.IsDigit(Convert.ToChar(lastdigit)))
                label1.Text = "";

            else
            {
                //we make a new character array of text so we can reverse and then set it back to text
                //we do this because it's possible that we're taking the square root of a number that's bigger than a single digit
                var c = text.ToCharArray();
                Array.Reverse(c);
                text = new string(c);

                //we find the index of the first space in the newly reverse string
                var index = text.Contains(" ") ? text.IndexOf(" ") : text.Length;

                //everything before the space that we found is our number
                var num = text.Substring(0, index);

                //now that we have our number, we need to reverse it so it's the correct way
                c = num.ToCharArray();
                Array.Reverse(c);
                num = new string(c);

                //we replace the number that wanted to be square rooted with the answer
                label1.Text = label1.Text.Substring(0, label1.Text.Length - index) + Math.Sqrt(Convert.ToDouble(num));
            }
        }

        //made keyboard shortcuts, so you can type out what you want instead of clicking buttons
        private void FrmCalc_KeyPress(object sender, KeyPressEventArgs e)
        {
            //e.KeyChar returns the ascii position of the character that was pressed

            //all digits, +, -, *, ^, /, e, E, (, )
            //enter/return key is not used because that's another way you can enter buttons so pressing E or e will do it instead
            if ((e.KeyChar >= 45 && e.KeyChar <= 57) || e.KeyChar == 94 || (e.KeyChar >= 40 && e.KeyChar <= 43) || e.KeyChar == 69 || e.KeyChar == 101)
                Buttonclick(sender, e);
            //c or C for Clear
            else if (e.KeyChar == 67 || e.KeyChar == 99)
                clear_Click(sender, e);
            //s or S for Square Root
            else if (e.KeyChar == 115 || e.KeyChar == 83)
                sqrt_Click(sender, e);
            //del or backspace for <--
            else if (e.KeyChar == 8 || e.KeyChar == 127)
                Delete_click(sender, e);
        }

        private void Delete_click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(label1.Text)) return;

            //we remove the last character
            label1.Text = label1.Text.Substring(0, label1.Text.Length - 1);
        }
    }
}
