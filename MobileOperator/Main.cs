using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MobileOperator
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        //Создаем экземпляр класса cLifeDB
        cLifeDB lifeConnect = new cLifeDB();
        //Переменная хранящая статус администратора
        public bool bAdminStatus = true;
        //Переменная хранящая тип авторизируемого пользователя
        public string sTypeOfUser = "";
        //Переменная не позволяющая проекту зависнуть в процессах
        public bool bCloseApp = true;
        //Переменная определяющая, добавляем или изменяем запись
        public bool bAdd = true;
        //Переменная хранящая имя пользователя
        public string sNameOfUser = "";
        
        //Загрузка формы
        private void Main_Load(object sender, EventArgs e)
        {
            //Строка подключения, для того, чтобы работало, нужно переписать значение Сервера и Названия БД
            lifeConnect.OpenConnection("Database=LifeDB;Server=ZICISE-PC;Trusted_Connection=True;MultipleActiveResultSets=True");

            if (sTypeOfUser == "Администратор")
            {
                //Выбор активной вкладки
                tabControlMain.SelectedTab = tabPageOperator;
                tabControlTables.SelectedTab = tabPageCallers;
                //Скрываем вкладку
                tabPageUser.Parent = null;

                //Загружаем таблицу
                lifeConnect.LoadTable("CALLERS", "SELECT C.iIdCallers AS 'ID Записи',  U.vName AS 'ФИО пользователя',"
                    + " C.vDateBirth AS 'Дата рождения', TP.vName AS 'Тарифный план', C.vNumerOfPhone AS 'Номер телефона',"
                    + " C.vSumm AS 'Личный счет', C.vDateContract AS 'Дата заключения контракта'"
                    + "FROM CALLERS AS C, USERS AS U, TARIFF_PLAN AS TP"
                    + " WHERE C.fk_iIdUsers = U.iIdUsers AND C.fk_iIdTariffPlan = TP.iIdTariffPlan"
                    , lifeConnect.binSourseCallers, dataGridViewCallers, bindingNavigatorOperator);

                //Загрузка выпадающих списков
                try
                {
                    lifeConnect.QueryToComboBox("SELECT vName FROM USERS WHERE iType = 2", comboBoxUsers, "vName");
                    lifeConnect.QueryToComboBox("SELECT vName FROM TARIFF_PLAN", comboBoxTariffPlanCallers, "vName");
                }
                catch { }
            }
            else if(sTypeOfUser == "Биллинг")
            {
                //Скрываем вкладки
                tabPageOperator.Parent = null;
                tabPageUser.Parent = null;
            }
            else
            {
                //Скрываем вкладки
                tabPageBiling.Parent = null;
                tabPageOperator.Parent = null;
            }

            helpProviderLife.HelpNamespace = @"Data\Help\Life_Help.chm";
            helpProviderLife.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProviderLife.SetShowHelp(this, true);
        }

        //Обработка события после закрытия формы
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (bCloseApp == true)
            {
                LogIn fLogIn = new LogIn();
                fLogIn.Show();
            }
        }

        /*Кнопки меню*/

        //Выход на форму авторизации
        private void авторизацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bCloseApp = false;
            Close();
            LogIn fLogIn = new LogIn();
            fLogIn.Show();
        }

        //Закрыть проект
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Вызвать информацию о проекте
        private void оПроектеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Программное средство «Мобильный оператор»\nПС «Оператор»\n- регистрация абонента и "
                + "тарифного плана/редактирование/удаление абонента"
                + "\n- регистрация тарифных планов/редактирование/удаление тарифного планая"
                + "\nПС «Биллинг»\n- возможность списывать условные деньги за услуги согласно тарифному плану абонента с его счета"
                + "\nПС пользователя\n- возможность смотреть балланс"
                + "\n- возможность пополнять балланс\nПС «Оператор»\n- возможность поменять тарифный план"
                + "\nПС пользователя\n- отчет выписка об изменениях в баллансе за заданный период времени."
                + "\n- учет рабочего времени", "Сообщение!");
        }

        //Вызов справки
        private void справкаF1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, helpProviderLife.HelpNamespace);
        }

        //Происходит при смене вкладки
        private void tabControlTables_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (tabControlTables.SelectedTab == tabPageCallers)
            {
                //Загружаем таблицу
                lifeConnect.LoadTable("CALLERS", "SELECT C.iIdCallers AS 'ID Записи',  U.vName AS 'ФИО пользователя',"
                    + " C.vDateBirth AS 'Дата рождения', TP.vName AS 'Тарифный план', C.vNumerOfPhone AS 'Номер телефона',"
                    + " C.vSumm AS 'Личный счет', C.vDateContract AS 'Дата заключения контракта'"
                    + "FROM CALLERS AS C, USERS AS U, TARIFF_PLAN AS TP"
                    + " WHERE C.fk_iIdUsers = U.iIdUsers AND C.fk_iIdTariffPlan = TP.iIdTariffPlan"
                    , lifeConnect.binSourseCallers, dataGridViewCallers, bindingNavigatorOperator);

                //Загрузка выпадающих списков
                try
                {
                    lifeConnect.QueryToComboBox("SELECT vName FROM USERS WHERE iType = 2", comboBoxUsers, "vName");
                    lifeConnect.QueryToComboBox("SELECT vName FROM TARIFF_PLAN", comboBoxTariffPlanCallers, "vName");
                }
                catch { }
            }
            else if (tabControlTables.SelectedTab == tabPageTariffPlan)
            {
                //Загружаем таблицу
                lifeConnect.LoadTable("TARIFF_PLAN", "SELECT iIdTariffPlan AS 'ID записи', vName AS 'Название тарифного плана',"
                    +" vDescription AS 'Описание тарифного плана', vSubscriptionFee AS 'Абонентская плата (руб)' FROM TARIFF_PLAN"
                    , lifeConnect.binSourseTariffPlan, dataGridViewTariffPlan, bindingNavigatorOperator);
            }
            else if (tabControlTables.SelectedTab == tabPageServices)
            {
                //Загружаем таблицу
                lifeConnect.LoadTable("SERVICES", "SELECT S.iIdServices AS 'ID записи', S.vName AS 'Название услуги',"
                    + " TP.vName AS 'Название тарифного плана', vPrice AS 'Стоимость услуги' FROM SERVICES AS S, TARIFF_PLAN AS TP"
                    + " WHERE S.fk_iIdTariffPlan = TP.iIdTariffPlan"
                    , lifeConnect.binSourseServices, dataGridViewServices, bindingNavigatorOperator);

                //Загрузка выпадающих списков
                try
                {
                    lifeConnect.QueryToComboBox("SELECT vName FROM TARIFF_PLAN", comboBoxTariffPlanServices, "vName");
                }
                catch { }
            }
            else
            {
                comboBoxTypeUsers.SelectedIndex = 0;

                //Загружаем таблицу
                lifeConnect.LoadTable("USERS", "SELECT iIdUsers AS 'ID записи', vName AS 'ФИО / Имя пользователя',"
                    + " vPassword AS 'Пароль пользователя', iType AS 'Тип пользователя(1-Адм,2-Бил,3-Пол)' FROM USERS"
                    , lifeConnect.binSourseUsers, dataGridViewUsers, bindingNavigatorOperator);
            }
        }

        /*Кнопки на форме*/

        //Открыть панель
        private void bOpen_Click(object sender, EventArgs e)
        {
            //Ставим флаг в true
            bAdd = true;

            if (tabControlTables.SelectedTab == tabPageCallers)
            {
                //Загружаем таблицу
                lifeConnect.LoadTable("CALLERS", "SELECT C.iIdCallers AS 'ID Записи',  U.vName AS 'ФИО пользователя',"
                    + " C.vDateBirth AS 'Дата рождения', TP.vName AS 'Тарифный план', C.vNumerOfPhone AS 'Номер телефона',"
                    + " C.vSumm AS 'Личный счет', C.vDateContract AS 'Дата заключения контракта'"
                    + "FROM CALLERS AS C, USERS AS U, TARIFF_PLAN AS TP"
                    + " WHERE C.fk_iIdUsers = U.iIdUsers AND C.fk_iIdTariffPlan = TP.iIdTariffPlan"
                    , lifeConnect.binSourseCallers, dataGridViewCallers, bindingNavigatorOperator);

                //Открываем панель
                panelCallers.Visible = true;
                labelCallers.Text = "Добавить запись";
            }
            else if (tabControlTables.SelectedTab == tabPageTariffPlan)
            {
                //Загружаем таблицу
                lifeConnect.LoadTable("TARIFF_PLAN", "SELECT iIdTariffPlan AS 'ID записи', vName AS 'Название тарифного плана',"
                    + " vDescription AS 'Описание тарифного плана', vSubscriptionFee AS 'Абонентская плата (руб)' FROM TARIFF_PLAN"
                    , lifeConnect.binSourseTariffPlan, dataGridViewTariffPlan, bindingNavigatorOperator);

                //Открываем панель
                panelTariffPlan.Visible = true;
                labelTariffPlan.Text = "Добавить запись";
            }
            else if (tabControlTables.SelectedTab == tabPageServices)
            {
                //Загружаем таблицу
                lifeConnect.LoadTable("SERVICES", "SELECT S.iIdServices AS 'ID записи', S.vName AS 'Название услуги',"
                    + " TP.vName AS 'Название тарифного плана', vPrice AS 'Стоимость услуги' FROM SERVICES AS S, TARIFF_PLAN AS TP"
                    + " WHERE S.fk_iIdTariffPlan = TP.iIdTariffPlan"
                    , lifeConnect.binSourseServices, dataGridViewServices, bindingNavigatorOperator);

                //Открываем панель
                panelServices.Visible = true;
                labelServices.Text = "Добавить запись";
            }
            else
            {
                //Загружаем таблицу
                lifeConnect.LoadTable("USERS", "SELECT iIdUsers AS 'ID записи', vName AS 'ФИО / Имя пользователя',"
                    + " vPassword AS 'Пароль пользователя', iType AS 'Тип пользователя(1-Адм,2-Бил,3-Пол)' FROM USERS"
                    , lifeConnect.binSourseUsers, dataGridViewUsers, bindingNavigatorOperator);

                //Открываем панель
                panelUsers.Visible = true;
                labelUsers.Text = "Добавить запись";
            }
        }

        //Открыть на редактирование
        private void bEdit_Click(object sender, EventArgs e)
        {
            //Ставим флаг в false
            bAdd = false;

            if (tabControlTables.SelectedTab == tabPageCallers)
            {
                int x = 0;

                try
                {
                    //Определяем индекс выбранной строки
                    x = dataGridViewCallers.CurrentRow.Index;
                }
                catch (Exception)
                {
                    MessageBox.Show("Редактировать нечего!", "Сообщение!");

                    return;
                }

                //Переносим данные на форму
                comboBoxUsers.Text = Convert.ToString(dataGridViewCallers[1, x].Value);
                dateTimePickerDateBirth.Text = Convert.ToString(dataGridViewCallers[2, x].Value);
                comboBoxTariffPlanCallers.Text = Convert.ToString(dataGridViewCallers[3, x].Value);
                maskedTextBoxPhoneNumber.Text = Convert.ToString(dataGridViewCallers[4, x].Value);
                numericUpDownSumm.Value = Convert.ToInt32(dataGridViewCallers[5, x].Value);
                dateTimePickerDateContract.Text = Convert.ToString(dataGridViewCallers[6, x].Value);

                //Открываем панель
                panelCallers.Visible = true;
                labelCallers.Text = "Изменить запись";
            }
            else if (tabControlTables.SelectedTab == tabPageTariffPlan)
            {
                int x = 0;

                try
                {
                    //Определяем индекс выбранной строки
                    x = dataGridViewTariffPlan.CurrentRow.Index;
                }
                catch (Exception)
                {
                    MessageBox.Show("Редактировать нечего!", "Сообщение!");

                    return;
                }

                //Переносим данные на форму
                textBoxNameTariffPlan.Text = Convert.ToString(dataGridViewTariffPlan[1, x].Value);
                textBoxDescTariffPlan.Text = Convert.ToString(dataGridViewTariffPlan[2, x].Value);
                numericUpDownSubFee.Value = Convert.ToInt32(dataGridViewTariffPlan[3, x].Value);

                //Открываем панель
                panelTariffPlan.Visible = true;
                labelTariffPlan.Text = "Изменить запись";
            }
            else if (tabControlTables.SelectedTab == tabPageServices)
            {
                int x = 0;

                try
                {
                    //Определяем индекс выбранной строки
                    x = dataGridViewServices.CurrentRow.Index;
                }
                catch (Exception)
                {
                    MessageBox.Show("Редактировать нечего!", "Сообщение!");

                    return;
                }

                //Переносим данные на форму
                textBoxNameService.Text = Convert.ToString(dataGridViewServices[1, x].Value);
                comboBoxTariffPlanServices.Text = Convert.ToString(dataGridViewServices[2, x].Value);
                numericUpDownPrice.Value = Convert.ToInt32(dataGridViewServices[3, x].Value);

                //Открываем панель
                panelServices.Visible = true;
                labelServices.Text = "Изменить запись";
            }
            else
            {
                int x = 0;

                try
                {
                    //Определяем индекс выбранной строки
                    x = dataGridViewUsers.CurrentRow.Index;
                }
                catch (Exception)
                {
                    MessageBox.Show("Редактировать нечего!", "Сообщение!");

                    return;
                }

                //Переносим данные на форму
                textBoxFIOUsers.Text = Convert.ToString(dataGridViewUsers[1, x].Value);
                textBoxPassUsers.Text = Convert.ToString(dataGridViewUsers[2, x].Value);
                comboBoxTypeUsers.Text = Convert.ToString(dataGridViewUsers[3, x].Value);

                //Открываем панель
                panelUsers.Visible = true;
                labelUsers.Text = "Изменить запись";
            }
        }

        //Открытие панели / удаление записи
        private void bDelete_Click(object sender, EventArgs e)
        {
            if (tabControlTables.SelectedTab == tabPageCallers)
            {
                //Если панель закрыта, удаляем запись
                if(panelCallers.Visible == false)
                {
                    if (MessageBox.Show("Вы действительно хотите удалить запись?",
                       "Сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (lifeConnect.QueryToBool("SELECT * FROM CALLERS") == false)
                        {
                            MessageBox.Show("Все строки были удалены из базы", "Ошибка удаления!");
                        }
                        else
                        {
                            int iIdNow = 0;
                            //Определяем индекс выбранной строки
                            int i = dataGridViewCallers.CurrentRow.Index;
                            iIdNow = Convert.ToInt32(dataGridViewCallers[0, i].Value);
                            //Удаление строки
                            lifeConnect.QueryToBool("DELETE FROM CALLERS WHERE iIdCallers = " + iIdNow);
                            //Очищаем таблицу отображения данных
                            lifeConnect.binSourseCallers.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    //Закрываем панель
                    panelCallers.Visible = false;
                    //Очищаем поля ввода
                    try
                    {
                        comboBoxUsers.SelectedIndex = 0;
                    }
                    catch { }
                    dateTimePickerDateBirth.Value = DateTime.Now;
                    try
                    {
                        comboBoxTariffPlanCallers.SelectedIndex = 0;
                    }
                    catch { }
                    maskedTextBoxPhoneNumber.Clear();
                    numericUpDownSumm.Value = 0;
                    dateTimePickerDateContract.Value = DateTime.Now;
                }

                //Загружаем таблицу
                lifeConnect.LoadTable("CALLERS", "SELECT C.iIdCallers AS 'ID Записи',  U.vName AS 'ФИО пользователя',"
                    + " C.vDateBirth AS 'Дата рождения', TP.vName AS 'Тарифный план', C.vNumerOfPhone AS 'Номер телефона',"
                    + " C.vSumm AS 'Личный счет', C.vDateContract AS 'Дата заключения контракта'"
                    + "FROM CALLERS AS C, USERS AS U, TARIFF_PLAN AS TP"
                    + " WHERE C.fk_iIdUsers = U.iIdUsers AND C.fk_iIdTariffPlan = TP.iIdTariffPlan"
                    , lifeConnect.binSourseCallers, dataGridViewCallers, bindingNavigatorOperator);
            }
            else if (tabControlTables.SelectedTab == tabPageTariffPlan)
            {
                //Если панель закрыта, удаляем запись
                if (panelTariffPlan.Visible == false)
                {
                    if (MessageBox.Show("Вы действительно хотите удалить запись?",
                       "Сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (lifeConnect.QueryToBool("SELECT * FROM TARIFF_PLAN") == false)
                        {
                            MessageBox.Show("Все строки были удалены из базы", "Ошибка удаления!");
                        }
                        else
                        {
                            int iIdNow = 0;
                            //Определяем индекс выбранной строки
                            int i = dataGridViewTariffPlan.CurrentRow.Index;
                            iIdNow = Convert.ToInt32(dataGridViewTariffPlan[0, i].Value);
                            //Удаление строки
                            lifeConnect.QueryToBool("DELETE FROM TARIFF_PLAN WHERE iIdTariffPlan = " + iIdNow);
                            //Очищаем таблицу отображения данных
                            lifeConnect.binSourseTariffPlan.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    //Закрываем панель
                    panelTariffPlan.Visible = false;
                    //Очищаем поля ввода
                    textBoxNameTariffPlan.Clear();
                    textBoxDescTariffPlan.Clear();
                    numericUpDownSubFee.Value = 0;
                }

                //Загружаем таблицу
                lifeConnect.LoadTable("TARIFF_PLAN", "SELECT iIdTariffPlan AS 'ID записи', vName AS 'Название тарифного плана',"
                    + " vDescription AS 'Описание тарифного плана', vSubscriptionFee AS 'Абонентская плата (руб)' FROM TARIFF_PLAN"
                    , lifeConnect.binSourseTariffPlan, dataGridViewTariffPlan, bindingNavigatorOperator);
            }
            else if (tabControlTables.SelectedTab == tabPageServices)
            {
                //Если панель закрыта, удаляем запись
                if (panelServices.Visible == false)
                {
                    if (MessageBox.Show("Вы действительно хотите удалить запись?",
                       "Сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (lifeConnect.QueryToBool("SELECT * FROM SERVICES") == false)
                        {
                            MessageBox.Show("Все строки были удалены из базы", "Ошибка удаления!");
                        }
                        else
                        {
                            int iIdNow = 0;
                            //Определяем индекс выбранной строки
                            int i = dataGridViewServices.CurrentRow.Index;
                            iIdNow = Convert.ToInt32(dataGridViewServices[0, i].Value);
                            //Удаление строки
                            lifeConnect.QueryToBool("DELETE FROM SERVICES WHERE iIdServices = " + iIdNow);
                            //Очищаем таблицу отображения данных
                            lifeConnect.binSourseServices.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    //Закрываем панель
                    panelServices.Visible = false;
                    //Очищаем поля ввода
                    textBoxNameService.Clear();
                    try
                    {
                        comboBoxTariffPlanServices.SelectedIndex = 0;
                    }
                    catch { }                    
                    numericUpDownPrice.Value = 0;
                }

                //Загружаем таблицу
                lifeConnect.LoadTable("SERVICES", "SELECT S.iIdServices AS 'ID записи', S.vName AS 'Название услуги',"
                    + " TP.vName AS 'Название тарифного плана' FROM SERVICES AS S, TARIFF_PLAN AS TP"
                    + " WHERE S.fk_iIdTariffPlan = TP.iIdTariffPlan"
                    , lifeConnect.binSourseServices, dataGridViewServices, bindingNavigatorOperator);
            }
            //Пользователи
            else
            {
                //Если панель закрыта, удаляем запись
                if (panelUsers.Visible == false)
                {
                    if (MessageBox.Show("Вы действительно хотите удалить запись?",
                       "Сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (lifeConnect.QueryToBool("SELECT * FROM USERS") == false)
                        {
                            MessageBox.Show("Все строки были удалены из базы", "Ошибка удаления!");
                        }
                        else
                        {
                            int iIdNow = 0;
                            //Определяем индекс выбранной строки
                            int i = dataGridViewUsers.CurrentRow.Index;
                            iIdNow = Convert.ToInt32(dataGridViewUsers[0, i].Value);
                            //Удаление строки
                            lifeConnect.QueryToBool("DELETE FROM USERS WHERE iIdUsers = " + iIdNow);
                            //Очищаем таблицу отображения данных
                            lifeConnect.binSourseUsers.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    //Закрываем панель
                    panelUsers.Visible = false;
                    //Очищаем поля ввода
                    textBoxFIOUsers.Clear();
                    textBoxPassUsers.Clear();
                    comboBoxTypeUsers.SelectedIndex = 0;
                }

                //Загружаем таблицу
                lifeConnect.LoadTable("USERS", "SELECT iIdUsers AS 'ID записи', vName AS 'ФИО / Имя пользователя',"
                    + " vPassword AS 'Пароль пользователя', iType AS 'Тип пользователя(1-Адм,2-Бил,3-Пол)' FROM USERS"
                    , lifeConnect.binSourseUsers, dataGridViewUsers, bindingNavigatorOperator);
            }
        }

        /*Вкладка операторы*/

        //Добавить запись
        private void bAddCallers_Click(object sender, EventArgs e)
        {
            //Проверка на добавление/изменение
            if (bAdd == true)
            {
                //Проверка не пустоту значений
                if (comboBoxUsers.Text.Equals("") || dateTimePickerDateBirth.Text.Equals("") || comboBoxTariffPlanCallers.Text.Equals("")
                    || maskedTextBoxPhoneNumber.Text.Equals("") || numericUpDownSumm.Value < 0 || dateTimePickerDateContract.Text.Equals(""))
                {
                    MessageBox.Show("Не все данные были введены!!!", "Сообщение!");
                }
                else
                {
                    int iId = 0;

                    try
                    {
                        //Получем ID максимальной записи
                        iId = Convert.ToInt32(dataGridViewCallers[0, dataGridViewCallers.Rows.Count - 1].Value);

                    }
                    catch (Exception)
                    {
                        iId++;
                    }

                    //Увеличиваем ID записи
                    iId++;

                    //Переменные для хранения ID записи пользователя и тарифного плана
                    int iIdUsers = 0, iIdTariffPlan = 0;
                    //Получение ID записи пользователя и тарифного плана
                    iIdUsers = Convert.ToInt32(lifeConnect.GetData("SELECT iIdUsers FROM USERS WHERE vName = '" + comboBoxUsers.Text + "'"));
                    iIdTariffPlan = Convert.ToInt32(lifeConnect.GetData("SELECT iIdTariffPlan FROM TARIFF_PLAN WHERE vName = '" 
                        + comboBoxTariffPlanCallers.Text + "'"));

                    //Проверка на добавление записи с одним и тем же ФИО и Тарифным планом
                    string sChechQuery = "SELECT * FROM CALLERS WHERE fk_iIdUsers = " + iIdUsers + " AND fk_iIdTariffPlan = " + iIdTariffPlan;
                    if(lifeConnect.QueryToBool(sChechQuery) == true)
                    {
                        MessageBox.Show("У этогот пользователя уже зарегестрирован даннный тарифный план!", "Сообщение!");
                        return;
                    }
                    else
                    {
                        //Добавляем новую запись
                        string sQueryAdd = "INSERT INTO CALLERS(iIdCallers, fk_iIdUsers, vDateBirth, fk_iIdTariffPlan, vNumerOfPhone, "
                            + "vSumm, vDateContract) VALUES("
                            + iId + "," + iIdUsers + ",'" + dateTimePickerDateBirth.Text + "'," + iIdTariffPlan + ",'" + maskedTextBoxPhoneNumber.Text
                            + "','" + numericUpDownSumm.Value.ToString() + "','" + dateTimePickerDateContract.Text + "');";

                        lifeConnect.QueryToBool(sQueryAdd);
                    }
                }
            }
            else
            {
                int iIdNow = 0;
                //Определяем индекс выбранной строки
                int i = dataGridViewCallers.CurrentRow.Index;
                iIdNow = Convert.ToInt32(dataGridViewCallers[0, i].Value);

                //Переменные для хранения ID записи пользователя и тарифного плана
                int iIdUsers = 0, iIdTariffPlan = 0;
                //Получение ID записи пользователя и тарифного плана
                iIdUsers = Convert.ToInt32(lifeConnect.GetData("SELECT iIdUsers FROM USERS WHERE vName = '" + comboBoxUsers.Text + "'"));
                iIdTariffPlan = Convert.ToInt32(lifeConnect.GetData("SELECT iIdTariffPlan FROM TARIFF_PLAN WHERE vName = '"
                    + comboBoxTariffPlanCallers.Text + "'"));

                //Редактируем существующую запись
                string sQueryEdit = "UPDATE CALLERS SET fk_iIdUsers = '" + iIdUsers + "', vDateBirth = '" + dateTimePickerDateBirth.Text
                        + "', fk_iIdTariffPlan = " + iIdTariffPlan + ", vNumerOfPhone = '" + maskedTextBoxPhoneNumber.Text
                        + "', vSumm = '" + numericUpDownSumm.Value.ToString() + "', vDateContract = '"
                        + dateTimePickerDateContract.Text + "' WHERE iIdCallers = " + iIdNow + ";";

                lifeConnect.QueryToBool(sQueryEdit);

                panelCallers.Visible = false;

                //Очищаем поля ввода
                try
                {
                    comboBoxUsers.SelectedIndex = 0;
                }
                catch { }
                dateTimePickerDateBirth.Value = DateTime.Now;
                try
                {
                    comboBoxTariffPlanCallers.SelectedIndex = 0;
                }
                catch { }
                maskedTextBoxPhoneNumber.Clear();
                numericUpDownSumm.Value = 0;
                dateTimePickerDateContract.Value = DateTime.Now;
            }

            //Загружаем таблицу
            lifeConnect.LoadTable("CALLERS", "SELECT C.iIdCallers AS 'ID Записи',  U.vName AS 'ФИО пользователя',"
                + " C.vDateBirth AS 'Дата рождения', TP.vName AS 'Тарифный план', C.vNumerOfPhone AS 'Номер телефона',"
                + " C.vSumm AS 'Личный счет', C.vDateContract AS 'Дата заключения контракта'"
                + "FROM CALLERS AS C, USERS AS U, TARIFF_PLAN AS TP"
                + " WHERE C.fk_iIdUsers = U.iIdUsers AND C.fk_iIdTariffPlan = TP.iIdTariffPlan"
                , lifeConnect.binSourseCallers, dataGridViewCallers, bindingNavigatorOperator);
        }

        //Очистить поля ввода
        private void bClearCallers_Click(object sender, EventArgs e)
        {
            try
            {
                comboBoxUsers.SelectedIndex = 0;
            }
            catch { }
            dateTimePickerDateBirth.Value = DateTime.Now;
            try
            {
                comboBoxTariffPlanCallers.SelectedIndex = 0;
            }
            catch { }
            maskedTextBoxPhoneNumber.Clear();
            numericUpDownSumm.Value = 0;
            dateTimePickerDateContract.Value = DateTime.Now;
        }

        /*Вкладка тарифные планы*/

        //Добавить запись
        private void bAddTariffPlan_Click(object sender, EventArgs e)
        {
            //Проверка на добавление/изменение
            if (bAdd == true)
            {
                //Проверка не пустоту значений
                if (textBoxNameTariffPlan.Text.Equals("") || textBoxDescTariffPlan.Text.Equals("") || numericUpDownSubFee.Value < 0)
                {
                    MessageBox.Show("Не все данные были введены!!!", "Сообщение!");
                }
                else
                {
                    int iId = 0;

                    try
                    {
                        //Получем ID максимальной записи
                        iId = Convert.ToInt32(dataGridViewTariffPlan[0, dataGridViewTariffPlan.Rows.Count - 1].Value);

                    }
                    catch (Exception)
                    {
                        iId++;
                    }

                    //Увеличиваем ID записи
                    iId++;

                    //Добавляем новую запись
                    string sQueryAdd = "INSERT INTO TARIFF_PLAN(iIdTariffPlan, vName, vDescription, vSubscriptionFee) VALUES("
                        + iId + ",'" + textBoxNameTariffPlan.Text + "','" + textBoxDescTariffPlan.Text + "','" 
                        + numericUpDownSubFee.Value.ToString() + "');";

                    lifeConnect.QueryToBool(sQueryAdd);
                }
            }
            else
            {
                int iIdNow = 0;
                //Определяем индекс выбранной строки
                int i = dataGridViewTariffPlan.CurrentRow.Index;
                iIdNow = Convert.ToInt32(dataGridViewTariffPlan[0, i].Value);

                //Редактируем существующую запись
                string sQueryEdit = "UPDATE TARIFF_PLAN SET vName = '" + textBoxNameTariffPlan.Text + "', vDescription = '" 
                    + textBoxDescTariffPlan.Text + "', vSubscriptionFee = '" + numericUpDownSubFee.Value.ToString()
                        + "' WHERE iIdTariffPlan = " + iIdNow + ";";

                lifeConnect.QueryToBool(sQueryEdit);

                panelTariffPlan.Visible = false;

                //Очищаем поля ввода
                textBoxNameTariffPlan.Clear();
                textBoxDescTariffPlan.Clear();
                numericUpDownSubFee.Value = 0;
            }

            //Загружаем таблицу
            lifeConnect.LoadTable("TARIFF_PLAN", "SELECT iIdTariffPlan AS 'ID записи', vName AS 'Название тарифного плана',"
                + " vDescription AS 'Описание тарифного плана', vSubscriptionFee AS 'Абонентская плата (руб)' FROM TARIFF_PLAN"
                , lifeConnect.binSourseTariffPlan, dataGridViewTariffPlan, bindingNavigatorOperator);
        }

        //Очистить поля ввода
        private void bClearTariffPlan_Click(object sender, EventArgs e)
        {
            //Очищаем поля ввода
            textBoxNameTariffPlan.Clear();
            textBoxDescTariffPlan.Clear();
            numericUpDownSubFee.Value = 0;
        }

        /*Вкладка услуги*/

        //Добавить запись
        private void bAddServices_Click(object sender, EventArgs e)
        {
            //Проверка на добавление/изменение
            if (bAdd == true)
            {
                //Проверка не пустоту значений
                if (textBoxNameService.Text.Equals("") || comboBoxTariffPlanServices.Text.Equals("") || numericUpDownPrice.Value < 0)
                {
                    MessageBox.Show("Не все данные были введены!!!", "Сообщение!");
                }
                else
                {
                    int iId = 0;

                    try
                    {
                        //Получем ID максимальной записи
                        iId = Convert.ToInt32(dataGridViewServices[0, dataGridViewServices.Rows.Count - 1].Value);

                    }
                    catch (Exception)
                    {
                        iId++;
                    }

                    //Переменные для хранения ID записи тарифного плана
                    int iIdTariffPlan = 0;
                    //Получение ID записи пользователя и тарифного плана
                    iIdTariffPlan = Convert.ToInt32(lifeConnect.GetData("SELECT iIdTariffPlan FROM TARIFF_PLAN WHERE vName = '" 
                        + comboBoxTariffPlanServices.Text + "'"));

                    //Увеличиваем ID записи
                    iId++;

                    //Добавляем новую запись
                    string sQueryAdd = "INSERT INTO SERVICES(iIdServices, vName, fk_iIdTariffPlan, vPrice) VALUES("
                        + iId + ",'" + textBoxNameService.Text + "','" + iIdTariffPlan + "','"
                        + numericUpDownPrice.Value.ToString() + "');";

                    lifeConnect.QueryToBool(sQueryAdd);
                }
            }
            else
            {
                int iIdNow = 0;
                //Определяем индекс выбранной строки
                int i = dataGridViewServices.CurrentRow.Index;
                iIdNow = Convert.ToInt32(dataGridViewServices[0, i].Value);

                //Переменные для хранения ID записи тарифного плана
                int iIdTariffPlan = 0;
                //Получение ID записи пользователя и тарифного плана
                iIdTariffPlan = Convert.ToInt32(lifeConnect.GetData("SELECT iIdTariffPlan FROM TARIFF_PLAN WHERE vName = '"
                    + comboBoxTariffPlanServices.Text + "'"));

                //Редактируем существующую запись
                string sQueryEdit = "UPDATE SERVICES SET vName = '" + textBoxNameService.Text + "', fk_iIdTariffPlan = " 
                    + iIdTariffPlan + ", vPrice = '" + numericUpDownPrice.Value.ToString()
                        + "' WHERE iIdServices = " + iIdNow + ";";

                lifeConnect.QueryToBool(sQueryEdit);

                panelServices.Visible = false;

                //Очищаем поля ввода
                textBoxNameService.Clear();
                try
                {
                    comboBoxTariffPlanServices.SelectedIndex = 0;
                }
                catch { }
                numericUpDownPrice.Value = 0;
            }

            //Загружаем таблицу
            lifeConnect.LoadTable("SERVICES", "SELECT S.iIdServices AS 'ID записи', S.vName AS 'Название услуги',"
                + " TP.vName AS 'Название тарифного плана', vPrice AS 'Стоимость услуги' FROM SERVICES AS S, TARIFF_PLAN AS TP"
                + " WHERE S.fk_iIdTariffPlan = TP.iIdTariffPlan"
                , lifeConnect.binSourseServices, dataGridViewServices, bindingNavigatorOperator);
        }

        //Очистить поля ввода
        private void bClearServices_Click(object sender, EventArgs e)
        {
            //Очищаем поля ввода
            textBoxNameService.Clear();
            try
            {
                comboBoxTariffPlanServices.SelectedIndex = 0;
            }
            catch { }
            numericUpDownPrice.Value = 0;
        }

        /*Вкладка пользователи*/

        //Добавить запись
        private void bAddUsers_Click(object sender, EventArgs e)
        {
            //Проверка на добавление/изменение
            if (bAdd == true)
            {
                //Проверка не пустоту значений
                if (textBoxFIOUsers.Text.Equals("") || textBoxPassUsers.Text.Equals(""))
                {
                    MessageBox.Show("Не все данные были введены!!!", "Сообщение!");
                }
                else
                {
                    int iId = 0;

                    try
                    {
                        //Получем ID максимальной записи
                        iId = Convert.ToInt32(dataGridViewUsers[0, dataGridViewUsers.Rows.Count - 1].Value);

                    }
                    catch (Exception)
                    {
                        iId++;
                    }

                    //Увеличиваем ID записи
                    iId++;

                    //Добавляем новую запись
                    string sQueryAdd = "INSERT INTO USERS(iIdUsers, vName, vPassword, iType) VALUES("
                        + iId + ",'" + textBoxFIOUsers.Text + "','" + textBoxPassUsers.Text + "','"
                        + comboBoxTypeUsers.SelectedIndex + "');";

                    lifeConnect.QueryToBool(sQueryAdd);
                }
            }
            else
            {
                int iIdNow = 0;
                //Определяем индекс выбранной строки
                int i = dataGridViewUsers.CurrentRow.Index;
                iIdNow = Convert.ToInt32(dataGridViewUsers[0, i].Value);

                //Редактируем существующую запись
                string sQueryEdit = "UPDATE USERS SET vName = '" + textBoxFIOUsers.Text + "', vPassword = '" + textBoxPassUsers.Text
                        + "', iType = '" + comboBoxTypeUsers.SelectedIndex + "' WHERE iIdUsers = " + iIdNow + ";";

                lifeConnect.QueryToBool(sQueryEdit);

                panelUsers.Visible = false;

                //Очищаем поля ввода
                textBoxFIOUsers.Clear();
                textBoxPassUsers.Clear();
                comboBoxTypeUsers.SelectedIndex = 0;
            }

            //Загружаем таблицу
            lifeConnect.LoadTable("USERS", "SELECT iIdUsers AS 'ID записи', vName AS 'ФИО / Имя пользователя',"
                + " vPassword AS 'Пароль пользователя', iType AS 'Тип пользователя(1-Адм,2-Бил,3-Пол)' FROM USERS"
                , lifeConnect.binSourseUsers, dataGridViewUsers, bindingNavigatorOperator);
        }

        //Очистить поля ввода
        private void bClearUsers_Click(object sender, EventArgs e)
        {
            //Очищаем поля ввода
            textBoxFIOUsers.Clear();
            textBoxPassUsers.Clear();
            comboBoxTypeUsers.SelectedIndex = 0;
        }

        /*Главный таб контрол*/

        //Происходит при смене вкладки
        private void tabControlMain_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageBiling)
            {
                //Очищаем поле отображения
                richTextBoxWriteOff.Clear();

                //Загрузка выпадающих списков
                try
                {
                    try
                    {
                        lifeConnect.QueryToComboBox("Select vName From USERS Where iType = 2", comboBoxBillingUser, "vName");
                    }
                    catch { }

                    //Устанавливаем значения в 0
                    comboBoxBillingUser.SelectedIndex = 0;
                    comboBoxBillingTariffPlan.SelectedIndex = 0;
                    comboBoxBillingService.SelectedIndex = 0;
                    //Лейбл с суммой делаем невидымым
                    labelBillingSumm.Visible = false;
                }
                catch { }
            }
            else if (tabControlMain.SelectedTab == tabPageUser)
            {
                //Очищаем поля отображения
                richTextBoxUserInf.Clear();
                numericUpDownUserBalance.Value = 0;
                dateTimePickerUserStart.Value = DateTime.Now;
                dateTimePickerUserFinish.Value = DateTime.Now;
                labelBalance.Text = "Текущий баланс:";
                //Очищаем таблицу
                dataGridViewUserWriteOf.DataSource = null;

                try
                {
                    //Получение ID пользователя
                    int iIdUsers = Convert.ToInt32(lifeConnect.GetData("SELECT iIdUsers FROM USERS WHERE vName = '" + sNameOfUser + "' AND iType = 2"));
                    //Получение тарифного плана пользователя
                    int iIdTariffPlan = Convert.ToInt32(lifeConnect.GetData("SELECT fk_iIdTariffPlan FROM CALLERS WHERE fk_iIdUsers = " + iIdUsers));
                    //Получение названия тарифного плана
                    string sTariifName = Convert.ToString(lifeConnect.GetData("SELECT vName FROM TARIFF_PLAN WHERE iIdTariffPlan = " + iIdTariffPlan));
                    //Получение даты заключение договора
                    string sDateOfContract = Convert.ToString(lifeConnect.GetData("SELECT vDateContract FROM CALLERS WHERE fk_iIdUsers = " + iIdUsers
                        + " AND fk_iIdTariffPlan = " + iIdTariffPlan));
                    //Получение номера телефона абонента
                    string sPhoneNumber = Convert.ToString(lifeConnect.GetData("SELECT vNumerOfPhone FROM CALLERS WHERE fk_iIdUsers = " + iIdUsers
                        + " AND fk_iIdTariffPlan = " + iIdTariffPlan));

                    richTextBoxUserInf.Text = "Данные пользователя:\nФИО: " + sNameOfUser + "\nТарифный план: " + sTariifName
                    + "\nДата заключения договора: " + sDateOfContract + "\nТелефонный номер: " + sPhoneNumber;
                }
                catch { }
            }
        }

        /*Вкладка 'ПС биллинг'*/

        //Происходит при смене значения выпадающего списка, загрузка тарифных планов
        private void comboBoxBillingUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Загрузка выпадающего списков
            try
            {
                //Получение ID пользователя
                int iIdUsers = Convert.ToInt32(lifeConnect.GetData("SELECT iIdUsers FROM USERS WHERE vName = '" + comboBoxBillingUser.Text + "'"));
                //Получение тарифного плана пользователя
                int iIdTariffPlan = Convert.ToInt32(lifeConnect.GetData("SELECT fk_iIdTariffPlan FROM CALLERS WHERE fk_iIdUsers = " + iIdUsers));

                lifeConnect.QueryToComboBox("SELECT vName FROM TARIFF_PLAN WHERE iIdTariffPlan = " + iIdTariffPlan, comboBoxBillingTariffPlan, "vName");
            }
            catch { }
        }

        //Происходит при смене значения выпадающего списка, загрузка услуг
        private void comboBoxBillingTariffPlan_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Загрузка выпадающего списков
            try
            {
                //Получение ID пользователя
                int iIdUsers = Convert.ToInt32(lifeConnect.GetData("SELECT iIdUsers FROM USERS WHERE vName = '" + comboBoxBillingUser.Text + "'"));
                //Получение тарифного плана пользователя
                int iIdTariffPlan = Convert.ToInt32(lifeConnect.GetData("SELECT fk_iIdTariffPlan FROM CALLERS WHERE fk_iIdUsers = " + iIdUsers));

                lifeConnect.QueryToComboBox("SELECT vName FROM SERVICES WHERE fk_iIdTariffPlan = " + iIdTariffPlan, comboBoxBillingService, "vName");
            }
            catch { }
        }

        //Процедура вызова максимального ID из таблица
        public int iMaxIdInTable(string sSqlQueryIsEmpty, string sSqlQueryMaxId)
        {
            int iMaxIdTableLocal = -1;

            //Если сушествуют данные в таблице, а это возврат true
            if (lifeConnect.QueryToBool(sSqlQueryIsEmpty) == true)
            {
                //То делаем запрос на max id в таблицу
                string sID = lifeConnect.GetData(sSqlQueryMaxId);
                try
                {
                    iMaxIdTableLocal = Convert.ToInt32(sID);
                    iMaxIdTableLocal++;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
            else
            {
                iMaxIdTableLocal = 1;
            }

            return iMaxIdTableLocal;
        }

        //Списать со счета услугу
        private void bWriteOfService_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите списать услугу?",
                       "Сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //Получение ID пользователя
                int iIdUsers = Convert.ToInt32(lifeConnect.GetData("SELECT iIdUsers FROM USERS WHERE vName = '" + comboBoxBillingUser.Text 
                    + "' AND iType = 2"));
                //Получение тарифного плана пользователя
                int iIdTariffPlan = Convert.ToInt32(lifeConnect.GetData("SELECT fk_iIdTariffPlan FROM CALLERS WHERE fk_iIdUsers = " + iIdUsers));

                //Получение суммы на счете пользователя
                int iSumm = Convert.ToInt32(lifeConnect.GetData("SELECT vSumm FROM CALLERS WHERE fk_iIdUsers = " + iIdUsers));
                //Получение стоимость услуги
                int iPriceService = Convert.ToInt32(lifeConnect.GetData("SELECT vPrice FROM SERVICES WHERE vName = '" + comboBoxBillingService.Text 
                    + "' AND fk_iIdTariffPlan = " + iIdTariffPlan));

                //Проверка на пустоту счета пользователя
                if(iSumm <= 0)
                {
                    MessageBox.Show("Счет пользователя пуст! Вы не можете больше списывать деньги с баланса.\nПополните баланс и повторите попытку позже"
                        ,"Сообщение!");
                    richTextBoxWriteOff.AppendText("Личный счет пользователя пуст!\n");
                    labelBillingSumm.Visible = true;
                    labelBillingSumm.Text = "Всего: " + iSumm + " руб.";
                    return;
                }

                //Списание суммы со счета
                iSumm -= iPriceService;

                labelBillingSumm.Visible = true;
                labelBillingSumm.Text = "Всего: " + iSumm + " руб.";

                //Получение id записи абонента
                int iIdCallers = Convert.ToInt32(lifeConnect.GetData("SELECT iIdCallers FROM CALLERS WHERE fk_iIdUsers = " 
                    + iIdUsers + " AND fk_iIdTariffPlan = " + iIdTariffPlan));

                //Изменить значение суммы на счете пользователя
                string sQueryEdit = "UPDATE CALLERS SET vSumm = '" + iSumm + "' WHERE iIdCallers = " + iIdCallers + ";";
                lifeConnect.QueryToBool(sQueryEdit);
                
                //Вызов процедуры получения максимального ID из таблицы, актоинкремент
                int iMaxIdTable = iMaxIdInTable("SELECT * FROM WRITE_OF_SERVICES", "SELECT MAX(iIdWriteOfServices) FROM WRITE_OF_SERVICES");

                //Заносим списание в специальную таблицу
                string sQueryAdd = "INSERT INTO WRITE_OF_SERVICES(iIdWriteOfServices, vFio, vDateOfWriteOff, vTariffPlan, "
                    +"vServices, vPrice, vSumm) VALUES("
                    + iMaxIdTable + ",'" + comboBoxBillingUser.Text + "','" + DateTime.Now + "','" + comboBoxBillingTariffPlan.Text + "','"
                    + comboBoxBillingService.Text + "','" + iPriceService + "','" + iSumm + "');";
                lifeConnect.QueryToBool(sQueryAdd);
                //Добавление в информационное поле
                richTextBoxWriteOff.AppendText("Списание со счета\nФИО: " + comboBoxBillingUser.Text + "\nДата списания: " + DateTime.Now
                    + "\nТарифный план: " + comboBoxBillingTariffPlan.Text + "\nУслуга: " + comboBoxBillingService.Text
                    + "\nСтоимость услуги: " + iPriceService + "\nСумма на счете после снятия: " + iSumm + "\n");
            }
        }

        /*Вкладка 'ПС Пользователя'*/

        //Узнать баланс на счете
        private void bUserBalance_Click(object sender, EventArgs e)
        {
            //Получение ID пользователя
            int iIdUsers = Convert.ToInt32(lifeConnect.GetData("SELECT iIdUsers FROM USERS WHERE vName = '" + sNameOfUser + "'"));
            //Получение суммы на счете пользователя
            int iSumm = Convert.ToInt32(lifeConnect.GetData("SELECT vSumm FROM CALLERS WHERE fk_iIdUsers = " + iIdUsers));
            labelBalance.Text = "Текущий баланс: " + iSumm + " руб.";
        }

        //Пополнение баланса на счете
        private void bUserAddMoney_Click(object sender, EventArgs e)
        {
            //Получение ID пользователя
            int iIdUsers = Convert.ToInt32(lifeConnect.GetData("SELECT iIdUsers FROM USERS WHERE vName = '" + sNameOfUser + "'"));
            //Получение тарифного плана пользователя
            int iIdTariffPlan = Convert.ToInt32(lifeConnect.GetData("SELECT fk_iIdTariffPlan FROM CALLERS WHERE fk_iIdUsers = " + iIdUsers));
            //Получение суммы на счете пользователя
            int iSumm = Convert.ToInt32(lifeConnect.GetData("SELECT vSumm FROM CALLERS WHERE fk_iIdUsers = " + iIdUsers));
            //Вынос в переменную суммы, на которую хотим увеличить наш счет
            int iAddSumm = Convert.ToInt32(numericUpDownUserBalance.Value);
            //Плюсуем это все
            iSumm += iAddSumm;
            //Получение id записи абонента
            int iIdCallers = Convert.ToInt32(lifeConnect.GetData("SELECT iIdCallers FROM CALLERS WHERE fk_iIdUsers = "
                + iIdUsers + " AND fk_iIdTariffPlan = " + iIdTariffPlan));
            //Изменить значение суммы на счете пользователя
            string sQueryEdit = "UPDATE CALLERS SET vSumm = '" + iSumm + "' WHERE iIdCallers = " + iIdCallers + ";";
            lifeConnect.QueryToBool(sQueryEdit);
            //Оповестить пользователя, что баланс был пополнен
            MessageBox.Show("Ваш баланс был успешно пополнен на: " + iAddSumm + " руб.", "Сообщение!");
        }

        //Прогрузка таблицы с выпиской
        private void bUserExtract_Click(object sender, EventArgs e)
        {
            string sQuery = "SELECT iIdWriteOfServices AS 'ID записи',"
                + " vFio AS 'ФИО пользователя', vDateOfWriteOff AS 'Дата списания', vTariffPlan AS 'Тарифный план',"
                + " vServices AS 'Услуги', vPrice AS 'Стоимость услуги', vSumm AS 'Сумма на счете' FROM WRITE_OF_SERVICES"
                + " WHERE vDateOfWriteOff >= '" + dateTimePickerUserStart.Text + "' AND vDateOfWriteOff <= '" 
                + dateTimePickerUserFinish.Text + "' AND vFio = '" + sNameOfUser + "'";

            //Загрузка выпыски
            lifeConnect.LoadTableWithoutNavigator("WRITE_OF_SERVICES", sQuery, lifeConnect.binSourseWriteOfServices, dataGridViewUserWriteOf);
        }
        
    }

    public class cLifeDB
    {
        public BindingSource binSourseUsers = new BindingSource();
        public BindingSource binSourseTariffPlan = new BindingSource();
        public BindingSource binSourseCallers= new BindingSource();
        public BindingSource binSourseServices = new BindingSource();
        public BindingSource binSourseWriteOfServices = new BindingSource();

        public DataTable dataTable;
        public SqlDataAdapter adap;
        public DataSet ds;
        public SqlCommandBuilder commBuild;

        private SqlConnection connect = null;

        //Процедура открытия конекта к базе данных
        public void OpenConnection(string connectionString)
        {
            connect = new SqlConnection(connectionString);
            try
            {
                connect.Open();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        //Процедура закрытия коннекта
        public void CloseConnection()
        {
            try
            {
                connect.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        //Процедура загрузки таблицы с учетом BindingNavigator
        public void LoadTable(string Name_Table, string queryString, BindingSource binSource, DataGridView dataGrid, BindingNavigator Navigator)
        {
            try
            {
                adap = new SqlDataAdapter(queryString, connect);
                ds = new DataSet();
                adap.Fill(ds, Name_Table);
                binSource.DataSource = ds.Tables[0];
                Navigator.BindingSource = binSource;
                dataGrid.DataSource = binSource;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Процедура загрузки таблицы
        public void LoadTableWithoutNavigator(string Name_Table, string queryString, BindingSource binSource, DataGridView dataGrid)
        {
            try
            {
                adap = new SqlDataAdapter(queryString, connect);
                ds = new DataSet();
                adap.Fill(ds, Name_Table);
                binSource.DataSource = ds.Tables[0];
                dataGrid.DataSource = binSource;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        //Процедура вывода в комбо бокса столбца таблицы
        public bool QueryToComboBox(string queryString, ComboBox comboBox, string Name_Column)
        {
            dataTable = new DataTable();
            SqlCommand com;
            SqlDataReader dataReader;
            com = new SqlCommand(queryString, connect);
            try
            {
                dataReader = com.ExecuteReader();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                return false;
            }
            if (dataReader.HasRows)
            {
                dataTable.Load(dataReader);
                comboBox.DataSource = dataTable;
                comboBox.DisplayMember = Name_Column;

                dataReader.Close();
                com.Dispose();
                return true;
            }
            dataReader.Close();
            com.Dispose();
            return false;
        }

        //Процедура для агрегатных запросов
        public string GetData(string queryString)
        {
            int iResultQuery = 0;
            string resultQuery = "";
            SqlCommand com;
            SqlDataReader dataReader;
            com = new SqlCommand(queryString, connect);
            try
            {
                dataReader = com.ExecuteReader();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return resultQuery;
            }

            dataReader.Read();

            //Пытаемся читать значение которое пришло, обычно это стринг, если нет, то ловим исключение
            try
            {
                resultQuery = dataReader.GetString(0);
            }
            //Поймали исключение и читаем не текст, а число
            catch (Exception)
            {
                //Необработанное исключение типа "System.InvalidOperationException" в System.Data.dll
                iResultQuery = dataReader.GetInt32(0);
                resultQuery = Convert.ToString(iResultQuery);
            }

            dataReader.Close();
            com.Dispose();
            return resultQuery;
        }

        //Вернет true, если есть записи
        public bool QueryToBool(string queryString)
        {
            dataTable = new DataTable();
            SqlCommand com;
            SqlDataReader dataReader;
            com = new SqlCommand(queryString, connect);
            try
            {
                dataReader = com.ExecuteReader();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                return false;
            }
            if (dataReader.HasRows)
            {
                dataTable.Load(dataReader);

                dataReader.Close();
                com.Dispose();
                return true;
            }
            dataReader.Close();
            com.Dispose();
            return false;
        }
    };
}