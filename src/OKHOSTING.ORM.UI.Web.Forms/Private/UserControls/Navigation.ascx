<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Navigation.ascx.cs" Inherits="OKHOSTING.ORM.UI.Web.Forms.Private.UserControls.Navigation" %>
<nav class="navbar-default">
	<ul>
		<li>
			Category 1
			<ul>
				<li>
					<a href="/Private/Person/List.aspx">Person</a>
				</li>
				<li>
					<a href="/Private/Country/List.aspx">Country</a>
				</li>
			</ul>
		</li>
		<li>
			Category 2
			<ul>
				<li>
					<a href="/Private/ZipCode/List.aspx">ZipCode</a>
				</li>
				<li>
					<a href="/Private/Sales/List.aspx">Sales</a>
				</li>
			</ul>
		</li>
		<li>
			User
			<ul>
				<li>
					<a href="/Private/User/ChangePassword.aspx">Change password</a>
				</li>
				<li>
					<a href="/Private/Logout.aspx">Logout</a>
				</li>
			</ul>
		</li>
	</ul>
</nav>
