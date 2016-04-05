using System;
using System.Data;
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
            }

            //if label1.text is blank then no mathmatical operations should be passed
            if (string.IsNullOrEmpty(label1.Text) && (!char.IsDigit(Convert.ToChar(text)) && text != @"."))
                //returns just break out of the function
                return;

            //can't have back to back mathmatical operations
            if ((!char.IsDigit(Convert.ToChar(text)) && text != @".") && label1.Text[label1.Text.Length - 1] == ' ')
                return;

            //if there's an equal sign then that means we already solved the equation so we should start from scratch
            if (label1.Text.Contains("="))
            {
                //I already have a clear button, might as well use the function so I'm not making duplicate code
                clear_Click(sender, e);
            }
            

            //if the button that's being selected is a digit then don't put spaces around it
            label1.Text += char.IsDigit(Convert.ToChar(text)) ? text : " " + text + " ";
            label1.Text = label1.Text.Replace(" . ", ".");

            //if there isn't an equal sign then just break out of the function
            if (!label1.Text.Contains("=")) return;

            //to use my evaluate function, there can't be an equal sign
            label1.Text = label1.Text.Replace("= ", "").Replace(" ^ ", "^");
            //with my all in one text writer, the periods get spaces around them, so we remove them

            //^ isn't a real mathmatical operation so I can't pass it through my evaluate function
            while (label1.Text.Contains("^"))
            {
                //since I know there is a ^ in my string, I need to find the index of it in the string
                var index = label1.Text.IndexOf("^");

                //since I the index of the ^, then before it should be the first number and after it will be the second number
                //By doing it this way I'm being returned a char and since those use the ascii value, if I didn't use
                //getnumericvalue, it would return me the ascii number instead of the decimal number
                var first = char.GetNumericValue(label1.Text[index - 1]);
                var second = char.GetNumericValue(label1.Text[index + 1]);

                //Math.Pow is how you do exponent calculations
                var equals = Convert.ToString(Math.Pow(first, second));

                //Now I need to edit my string to add in the change so the Evaluate function can understand it
                label1.Text = label1.Text.Replace(first + "^" + second, equals);
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

            //all digits, +, -, *, ^, /, e, E
            //enter/return key is not used because that's another way you can enter buttons so pressing E or e will do it instead
            if ((e.KeyChar >= 45 && e.KeyChar <= 57) || e.KeyChar == 94 || e.KeyChar == 43 || e.KeyChar == 42 || e.KeyChar == 69 || e.KeyChar == 101)
                Buttonclick(sender, e);
            //c or C for Clear
            else if (e.KeyChar == 67 || e.KeyChar == 99)
                clear_Click(sender, e);
            //s or S for Square Root
            else if (e.KeyChar == 115 || e.KeyChar == 83)
                sqrt_Click(sender, e);
        }
    }
}
