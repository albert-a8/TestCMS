<%@ Control %>
<%@ Register Assembly="Telerik.Sitefinity" TagPrefix="sf" Namespace="Telerik.Sitefinity.Web.UI" %>
<%@ Register Assembly="Telerik.Sitefinity" TagPrefix="sitefinity" Namespace="Telerik.Sitefinity.Web.UI" %>
<%@ Register Assembly="Telerik.Sitefinity" TagPrefix="sfFields" Namespace="Telerik.Sitefinity.Web.UI.Fields" %>

<sitefinity:ResourceLinks ID="resourcesLinks" runat="server">
    <sitefinity:ResourceFile Name="Styles/Ajax.css" />
</sitefinity:ResourceLinks>
<div id="designerLayoutRoot" class="sfContentViews sfSingleContentView" style="max-height: 400px; overflow: auto; ">
<ol>        
    <li class="sfFormCtrl">
    <asp:Label runat="server" AssociatedControlID="PageName" CssClass="sfTxtLbl">Page Name</asp:Label>
    <asp:TextBox ID="PageName" runat="server" CssClass="sfTxt" />
    <div class="sfExample">Redirect link page name</div>
    </li> 
    <li class="sfFormCtrl">
    <asp:Label runat="server" AssociatedControlID="PageURL" CssClass="sfTxtLbl">Page URL</asp:Label>
    <asp:TextBox ID="PageURL" runat="server" CssClass="sfTxt" />
    <div class="sfExample">Redirect link URL</div>
    </li>
</ol>
</div>
