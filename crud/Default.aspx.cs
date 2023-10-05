using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI.WebControls;

namespace crud
{
    public partial class _Default : System.Web.UI.Page
    {

        private SortDirection CurrentSortDirection
        {
            get
            {
                if (ViewState["sortDirection"] == null)
                {
                    ViewState["sortDirection"] = SortDirection.Ascending;
                }
                return (SortDirection)ViewState["sortDirection"];
            }
            set
            {
                ViewState["sortDirection"] = value;
            }
        }
        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
            }
        }

        #endregion

        #region Button Click Events

        protected void ADDBUTTON_Click(object sender, EventArgs e)
        {
            ShowUserDetailsForm("ADD USER", "Save");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            ShowGridView();
            ResetForm();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text;
                string email = txtEmail.Text;
                string mobile = txtMobile.Text;
                string country = ddlCountry.SelectedValue;
                string gender = rblGender.SelectedValue;
                bool status = chkStatus.Checked;

                if (btnSave.Text == "Save")
                {
                    if (IsEmailExists(email))
                    {
                        lblValidationMessage.Text = "Email already exists!";
                        return;
                    }

                    InsertUserData(name, email, country, mobile, gender, status);
                }
                else
                {
                    UpdateUserData(name, email, country, mobile, gender, status);
                }

                ClearControls();
                ResetForm();
                BindGridView();
            }
            catch (Exception ex)
            {
                lblValidationMessage.Text = "An error occurred: " + ex.Message;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (btnSave.Text == "Save")
            {
                ResetForm();
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToCsv();
            }
            catch (Exception ex)
            {
                lblValidationMessage.Text = "An error occurred: " + ex.Message;
            }
        }

        #endregion

        #region GridView Events

        protected void lnk_onClick(object sender, EventArgs e)
        {
            try
            {
                int userID = Convert.ToInt32((sender as Button).CommandArgument);
                ShowUserDetailsFormForEdit(userID);
            }
            catch (Exception ex)
            {
                lblValidationMessage.Text = "An error occurred: " + ex.Message;
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int rowIndex = e.RowIndex;
                int userID = Convert.ToInt32(GridView1.DataKeys[rowIndex].Value);
                DeleteUserData(userID);
                BindGridView();
            }
            catch (Exception ex)
            {
                lblValidationMessage.Text = "An error occurred: " + ex.Message;
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                GridView1.PageIndex = e.NewPageIndex;
                BindGridView();
            }
            catch (Exception ex)
            {
                lblValidationMessage.Text = "An error occurred: " + ex.Message;
            }
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                SortGridView(e.SortExpression);
            }
            catch (Exception ex)
            {
                lblValidationMessage.Text = "An error occurred: " + ex.Message;
            }
        }

        #endregion

        #region TextChanged Events

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SearchAndBindGridView(txtSearch.Text.Trim());
            }
            catch (Exception ex)
            {
                lblValidationMessage.Text = "An error occurred: " + ex.Message;
            }
        }

        #endregion

        #region Dropdown Events

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ChangePageSize();

                // Check the count of items in the GridView
                //if (GridView1.Rows.Count <= int.Parse(ddlPageSize.SelectedValue))
                //{
                //    // Hide the page size dropdown if the count is less than or equal to the selected page size
                //    ddlPageSize.Visible = false;
                //}
                //else
                //{
                //    ddlPageSize.Visible = true;
                //}
            }
            catch (Exception ex)
            {
                lblValidationMessage.Text = "An error occurred: " + ex.Message;
            }
        }


        #endregion

        #region Helper Methods

        private void InitializePage()
        {
            userDetailsForm.Visible = false;
            GridViewFull.Visible = true;
            lblValidationMessage.Text = null;
            btnSave.Text = "Save";
            userAddFormTitle.InnerText = "ADD USER";

            List<string> countryNames = GetCountryList();
            ddlCountry.DataSource = countryNames;
            ddlCountry.DataBind();
            BindGridView();
        }

        private void ShowUserDetailsForm(string title, string buttonText)
        {
            userDetailsForm.Visible = true;
            GridViewFull.Visible = false;
            btnSave.Text = buttonText;
            userAddFormTitle.InnerText = title;
        }

        private void ShowUserDetailsFormForEdit(int userID)
        {
            ShowUserDetailsForm("Edit USER", "Update");
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM UserDetails WHERE UserID = @UserID", con))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.SelectCommand.Parameters.AddWithValue("@UserID", userID);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                    hiddenUserID.Value = userID.ToString();
                    txtName.Text = dt.Rows[0]["Name"].ToString();
                    txtMobile.Text = dt.Rows[0]["MobileNumber"].ToString();
                    txtEmail.Text = dt.Rows[0]["Email"].ToString();
                    string selectedCountry = dt.Rows[0]["Country"].ToString();
                    ddlCountry.SelectedValue = selectedCountry;

                    string selectedGender = dt.Rows[0]["Gender"].ToString();
                    rblGender.SelectedValue = selectedGender;

                    bool status = Convert.ToBoolean(dt.Rows[0]["Status"]);
                    chkStatus.Checked = status;
                }
            }
        }

        private bool IsEmailExists(string email)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM UserDetails WHERE Email = @Email", con))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private void InsertUserData(string name, string email, string country, string mobile, string gender, bool status)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("InsertUserDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Country", country);
                cmd.Parameters.AddWithValue("@MobileNumber", mobile);
                cmd.Parameters.AddWithValue("@Gender", gender);
                cmd.Parameters.AddWithValue("@Status", status);

                con.Open();
                cmd.ExecuteNonQuery();
                userDetailsForm.Visible = false;
                GridViewFull.Visible = true;
            }
        }

        private void UpdateUserData(string name, string email, string country, string mobile, string gender, bool status)
        {
            int userID = Convert.ToInt32(hiddenUserID.Value);
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("UPDATE UserDetails SET Name = @Name, Email = @Email, Country = @Country, MobileNumber = @MobileNumber, Gender = @Gender, Status = @Status WHERE UserID = @UserID", con))
            {
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Country", country);
                cmd.Parameters.AddWithValue("@MobileNumber", mobile);
                cmd.Parameters.AddWithValue("@Gender", gender);
                cmd.Parameters.AddWithValue("@Status", status);

                con.Open();
                cmd.ExecuteNonQuery();

                userDetailsForm.Visible = false;
                GridViewFull.Visible = true;
            }

            ResetForm();
            BindGridView();
        }

        private void DeleteUserData(int userID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("DELETE FROM UserDetails WHERE UserID = @UserID", con))
            {
                cmd.Parameters.AddWithValue("@UserID", userID);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            BindGridView();
        }

        private void SortGridView(string sortExpression)
        {
            DataTable dt = ViewState["Paging"] as DataTable;

            if (dt != null)
            {
                dt.DefaultView.Sort = sortExpression + " " + (CurrentSortDirection == SortDirection.Ascending ? "ASC" : "DESC");
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }

            CurrentSortDirection = GetNextSortDirection(CurrentSortDirection);
        }

        private SortDirection GetNextSortDirection(SortDirection currentSortDirection)
        {
            return currentSortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
        }

        private void ExportToCsv()
        {
            DataTable exportData = GetExportData();
            string csvData = GenerateCsv(exportData);

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=exported_data.csv");
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.Output.Write(csvData);
            Response.Flush();
            Response.End();
        }

        private DataTable GetExportData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM UserDetails", con))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        private string GenerateCsv(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataColumn column in dt.Columns)
            {
                if (column.ColumnName == "MobileNumber")
                {
                    sb.Append("Mobile Number");
                }
                else if (column.ColumnName == "Status")
                {
                    sb.Append("Status");
                }
                else
                {
                    sb.Append(column.ColumnName);
                }
                sb.Append(",");
            }
            sb.AppendLine();

            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    if (column.ColumnName == "MobileNumber")
                    {
                        sb.Append(row[column].ToString());
                    }
                    else if (column.ColumnName == "Status")
                    {
                        bool status = Convert.ToBoolean(row[column]);
                        sb.Append(status ? "Active" : "Inactive");
                    }
                    else
                    {
                        sb.Append(row[column].ToString());
                    }
                    sb.Append(",");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private List<string> GetCountryList()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT country_name FROM countries", con))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    List<string> countryList = new List<string>();

                    foreach (DataRow row in dt.Rows)
                    {
                        countryList.Add(row["country_name"].ToString());
                    }

                    return countryList;
                }
            }
        }

        private void SearchAndBindGridView(string searchKeyword)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT UserID, Name, Email, Country, MobileNumber, Gender, Status FROM UserDetails WHERE Name LIKE @SearchKeyword OR Email LIKE @SearchKeyword OR Country LIKE @SearchKeyword OR MobileNumber LIKE @SearchKeyword", con))
            {
                cmd.Parameters.AddWithValue("@SearchKeyword", "%" + searchKeyword + "%");

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
            }
        }

        private void ChangePageSize()
        {
            int newPageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            GridView1.PageSize = newPageSize;
            BindGridView();
        }

        private void BindGridView()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                //string query = "SELECT UserID, Name, Email, Country, MobileNumber, Gender, Status FROM UserDetails " +
                //    "ORDER BY Name OFFSET @PageOffset ROWS FETCH NEXT @PageSize ROWS ONLY";
                string query = "SELECT UserID, Name, Email, Country, MobileNumber, Gender, Status FROM UserDetails " ;

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    //int currentPageIndex = GridView1.PageIndex;
                    //int pageSize = GridView1.PageSize;
                    //int pageOffset = currentPageIndex * pageSize;

                    //cmd.Parameters.AddWithValue("@PageOffset", pageOffset);
                    //cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                    }
                }
            }
        }

        private void ClearControls()
        {
            txtName.Text = "";
            txtEmail.Text = "";
            txtMobile.Text = "";
            ddlCountry.SelectedIndex = 0;
            rblGender.ClearSelection();
            chkStatus.Checked = false;
            lblValidationMessage.Text = null;
        }

        private void ResetForm()
        {
            ClearControls();
            ShowGridView();
        }

        private void ShowGridView()
        {
            userDetailsForm.Visible = false;
            GridViewFull.Visible = true;
            BindGridView();
            lblValidationMessage.Text = null;
        }

        #endregion
    }
}
