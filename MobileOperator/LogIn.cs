using System;
using System.Windows.Forms;

namespace MobileOperator
{
    public partial class LogIn : Form
    {
        public LogIn()
        {
            InitializeComponent();
        }

        //Создаем экземпляр класса cLifeDB
        cLifeDB lifeConnect = new cLifeDB();

        //Войти в программу
        private void bEnter_Click(object sender, EventArgs e)
        {
            string sQueryIsEmpry;

            //Проверка типа пользователя, бухгалтерия
            if (comboBoxType.SelectedIndex == 0)
            {
                sQueryIsEmpry = "SELECT * FROM USERS WHERE iType = 0";
                //Проверка на существование в базе кого-нибудь из администрации
                if (lifeConnect.QueryToBool(sQueryIsEmpry) == true)
                {
                    //Проверка не ввод данных
                    if (tBNameUser.Text.Equals("") || tBPassword.Text.Equals(""))
                    {
                        MessageBox.Show("Вы не ввели все данные!!!", "Внимание!!!");
                    }
                    else
                    {
                        //Строка проверки
                        sQueryIsEmpry = "SELECT * FROM USERS WHERE vName = '" + tBNameUser.Text + "' AND vPassword = '"
                            + tBPassword.Text + "' AND iType = 0;";
                        //Проверка на существование аккаунта
                        if (lifeConnect.QueryToBool(sQueryIsEmpry) == true)
                        {
                            Main main = new Main();
                            main.bAdminStatus = true;
                            main.sTypeOfUser = "Администратор";
                            main.Text = "Мобильный оператор Life:) - Администратор (" + tBNameUser.Text + ")";
                            main.Show();
                            Hide();
                        }
                        else
                        {
                            MessageBox.Show("Пароль не верен, повторите попытку!!!", "Внимание!!!");
                        }
                    }
                }
                else
                {
                    //Предложение создать администратора с паролем по умолчанию
                    if (MessageBox.Show("Аккаунта администратора не существует!\nСоздать по умолчанию?\nИмя: admin\nПароль:admin",
                       "Сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        sQueryIsEmpry = "INSERT INTO USERS (iIdUsers, vName, vPassword, iType) VALUES (0, 'admin', 'admin', 0);";
                        //Внос значения в базу данных
                        lifeConnect.QueryToBool(sQueryIsEmpry);
                    }

                    else
                    {
                        Application.Exit();
                    }
                }
            }
            //Пользователь
            else if (comboBoxType.SelectedIndex == 2)
            {
                //Проверка не ввод данных
                if (tBNameUser.Text.Equals("") || tBPassword.Text.Equals(""))
                {
                    MessageBox.Show("Вы не ввели все данные!!!", "Внимание!!!");
                }
                else
                {
                    //Строка проверки
                    sQueryIsEmpry = "SELECT * FROM USERS WHERE vName = '" + tBNameUser.Text + "' AND vPassword = '"
                        + tBPassword.Text + "' AND iType = 2;";
                    //Проверка на существование аккаунта
                    if (lifeConnect.QueryToBool(sQueryIsEmpry) == true)
                    {
                        Main main = new Main();
                        main.bAdminStatus = false;
                        main.sTypeOfUser = "Пользователь";
                        main.sNameOfUser = tBNameUser.Text;
                        main.Text = "Мобильный оператор Life:) - Пользователь (" + tBNameUser.Text + ")";
                        main.Show();
                        Hide();
                    }
                    else
                    {
                        MessageBox.Show("Пароль не верен, повторите попытку!!!", "Внимание!!!");
                    }
                }
            }
            //Биллинг
            else
            {
                //Проверка не ввод данных
                if (tBNameUser.Text.Equals("") || tBPassword.Text.Equals(""))
                {
                    MessageBox.Show("Вы не ввели все данные!!!", "Внимание!!!");
                }
                else
                {
                    //Строка проверки
                    sQueryIsEmpry = "SELECT * FROM USERS WHERE vName = '" + tBNameUser.Text + "' AND vPassword = '"
                        + tBPassword.Text + "' AND iType = 1;";
                    //Проверка на существование аккаунта
                    if (lifeConnect.QueryToBool(sQueryIsEmpry) == true)
                    {
                        Main main = new Main();
                        main.bAdminStatus = false;
                        main.sTypeOfUser = "Биллинг";
                        main.Text = "Мобильный оператор Life:) - Биллинг (" + tBNameUser.Text + ")";
                        main.Show();
                        Hide();
                    }
                    else
                    {
                        MessageBox.Show("Пароль не верен, повторите попытку!!!", "Внимание!!!");
                    }
                }
            }
        }

        //Закрыть весь проект
        private void bClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
        //Загрузка формы
        private void LogIn_Load(object sender, EventArgs e)
        {
            //Выбор по умолчанию администратора
            comboBoxType.SelectedIndex = 0;
            //Блочим форму, пока идет проверка настроек
            Enabled = false;
            //Устанавливаем текст для лейбла
            ConnectionLabel.Text = "Проверяем настройки...";
            //Интервал в 1 секунду
            timer1.Interval = 1000;
            //Запуск таймера
            timer1.Start();
        }

        //Действия на таймере
        private void timer1_Tick(object sender, EventArgs e)
        {
            //Пытаемся присоединиться к серверу
            try
            {
                lifeConnect.OpenConnection("Database=LifeDB;Server=ZICISE;Trusted_Connection=True;MultipleActiveResultSets=True");
                //Выводим форму из блокировки
                Enabled = true;
                //Устанавливаем текст для лейбла
                ConnectionLabel.Text = "Настройки верные!!!";
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                return;
            }
        }
    }
}