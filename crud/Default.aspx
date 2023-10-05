<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="crud._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div id="userDetailsForm" runat="server" class="user-details-form" style="box-shadow: 10px 10px 5px lightblue; width: 25%; margin: auto;padding:10px;border-radius:7px;border:1px solid #ccc;">
        <h2 class="form-title" style="display: inline-block; margin-right: 10px;" id="userAddFormTitle" runat="server"></h2>
        <input type="hidden" id="hiddenUserID" runat="server" />

        <div class="form-group">
            <label for="txtName" class="form-label">Name:</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Enter your name" />
            <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                InitialValue="" ErrorMessage="Name is required." ForeColor="Red" Display="Dynamic" />
        </div>

        <div class="form-group">
            <label for="txtEmail" class="form-label">Email:</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter your email" />
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                InitialValue="" ErrorMessage="Email is required." ForeColor="Red" Display="Dynamic" />
        </div>

        <div class="form-group">
            <label for="txtMobile" class="form-label">Mobile:</label>
            <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" placeholder="Enter your mobile number" />
            <asp:RequiredFieldValidator ID="rfvMobile" runat="server" ControlToValidate="txtMobile"
                InitialValue="" ErrorMessage="Mobile is required." ForeColor="Red" Display="Dynamic" />
            <asp:RegularExpressionValidator ID="revMobile" runat="server" ControlToValidate="txtMobile"
                ValidationExpression="^\d{1,10}$"
                ErrorMessage="Mobile number must be a maximum of 10 digits." ForeColor="Red" Display="Dynamic" />
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtMobile"
                ValidationExpression="^\d{10}$"
                ErrorMessage="Mobile number must be exactly 10 digits." ForeColor="Red" Display="Dynamic" />
        </div>

        <div class="form-group">
            <label for="ddlCountry" class="form-label">Country:</label>
            <asp:DropDownList ID="ddlCountry" runat="server" CssClass="form-control">
                <asp:ListItem Text="Select Country" Value="" />
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvCountry" runat="server" ControlToValidate="ddlCountry"
                InitialValue="" ErrorMessage="Country is required." ForeColor="Red" Display="Dynamic" />
        </div>

        <div class="form-group">
            <label class="form-label">Gender:</label>
            <asp:RadioButtonList ID="rblGender" runat="server" CssClass="form-check">
                <asp:ListItem Text="Male" Value="Male" />
                <asp:ListItem Text="Female" Value="Female" />
                <asp:ListItem Text="Other" Value="Other" />
            </asp:RadioButtonList>
            <asp:RequiredFieldValidator ID="rfvGender" runat="server" ControlToValidate="rblGender"
                InitialValue="" ErrorMessage="Gender is required." ForeColor="Red" Display="Dynamic" />
        </div>

        <div class="form-group">
            <label class="form-label">Status:</label>
            <asp:CheckBox ID="chkStatus" runat="server" Checked="true" />
        </div>

        <div class="form-group">
            <asp:Label ID="lblValidationMessage" class="text-danger" runat="server"></asp:Label>
        </div>

        <div class="form-group">
            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-primary" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" CssClass="btn btn-secondary" CausesValidation="false" />
            <asp:Button ID="btnBack" runat="server" Text="Back" OnClick="btnBack_Click" CssClass="btn btn-secondary" CausesValidation="false" />
        </div>
    </div>

    <div id="GridViewFull" runat="server">
        <h2 style="display: inline-block; margin-right: 10px;">Users List</h2>

        <div class="row" style="display: inline-block; float: right; padding: 15px; line-height: 3;">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search" OnTextChanged="txtSearch_TextChanged" AutoPostBack="True" />
            <asp:Button ID="ADDBUTTON" runat="server" Text="ADD USER" CssClass="btn btn-success" OnClick="ADDBUTTON_Click" OnClientClick="changeFormTitle('ADD USER');" />
            <asp:Button ID="btnExport" runat="server" Text="Export" OnClick="btnExport_Click" CssClass="btn btn-primary" />
        </div>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" DataKeyNames="UserID" CssClass="table table-striped"
            OnRowDeleting="GridView1_RowDeleting" AllowSorting="true" OnSorting="GridView1_Sorting" AllowPaging="true" PageSize="5"
            OnPageIndexChanging="GridView1_PageIndexChanging">
            <Columns>
                <asp:TemplateField HeaderText="Sl No" ItemStyle-CssClass="text-center">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="UserID" HeaderText="User ID" ReadOnly="true" SortExpression="UserID" HeaderStyle-CssClass="sort-header" />
                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" HeaderStyle-CssClass="sort-header"/>
                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" HeaderStyle-CssClass="sort-header" />
                <asp:BoundField DataField="Country" HeaderText="Country" SortExpression="Country" HeaderStyle-CssClass="sort-header" />
                <asp:BoundField DataField="MobileNumber" HeaderText="Mobile Number" SortExpression="MobileNumber" HeaderStyle-CssClass="sort-header" />
                <asp:BoundField DataField="Gender" HeaderText="Gender" SortExpression="Gender" HeaderStyle-CssClass="sort-header" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" Text='<%# (bool)Eval("Status") ? "Active" : "Inactive" %>'
                            ForeColor='<%# (bool)Eval("Status") ? System.Drawing.Color.Green : System.Drawing.Color.Red %>'>
                        </asp:Label>
                    </ItemTemplate>

                </asp:TemplateField>

                <asp:TemplateField HeaderText="Actions">

                    <ItemTemplate>
                        <%--<asp:LinkButton ID="lnkView" runat="server" CommandArgument='<%# Eval("UserID")%>' OnClick="lnk_onClick">Edit</asp:LinkButton>--%>
                        <asp:Button ID="lnkView" runat="server" Text="Edit" CommandArgument='<%# Eval("UserID")%>' OnClick="lnk_onClick" CssClass="btn btn-warning" OnClientClick="changeFormTitle('EDIT USER');" />
                        <asp:Button runat="server" Text="Delete" CommandName="Delete" CssClass="btn btn-danger mr-2"
                            OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <div class="dropdowns" id="drodown">
           <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
            <asp:ListItem Text="5" Value="5" />
            <asp:ListItem Text="10" Value="10" />
            <asp:ListItem Text="15" Value="15" />
            <asp:ListItem Text="20" Value="20" />
            <asp:ListItem Text="25" Value="25" />
            <asp:ListItem Text="30" Value="30" />
            <asp:ListItem Text="35" Value="35" />
            <asp:ListItem Text="40" Value="40" />
        </asp:DropDownList>
        </div>

        <!-- Empty div to display search results -->
        <div id="searchResults" runat="server"></div>
    </div>

</asp:Content>
