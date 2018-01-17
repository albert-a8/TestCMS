Type.registerNamespace("SitefinityWebApp.WidgetDesigners.RedirectionLink");

SitefinityWebApp.WidgetDesigners.RedirectionLink.RedirectionLinkDesigner = function (element) {
    /* Initialize Message fields */
    this._message = null;
    
    /* Calls the base constructor */
    SitefinityWebApp.WidgetDesigners.RedirectionLink.RedirectionLinkDesigner.initializeBase(this, [element]);
}

SitefinityWebApp.WidgetDesigners.RedirectionLink.RedirectionLinkDesigner.prototype = {
    /* --------------------------------- set up and tear down --------------------------------- */
    initialize: function () {
        /* Here you can attach to events or do other initialization */
        SitefinityWebApp.WidgetDesigners.RedirectionLink.RedirectionLinkDesigner.callBaseMethod(this, 'initialize');
    },
    dispose: function () {
        /* this is the place to unbind/dispose the event handlers created in the initialize method */
        SitefinityWebApp.WidgetDesigners.RedirectionLink.RedirectionLinkDesigner.callBaseMethod(this, 'dispose');
    },

    /* --------------------------------- public methods ---------------------------------- */

    findElement: function (id) {
        var result = jQuery(this.get_element()).find("#" + id).get(0);
        return result;
    },

    /* Called when the designer window gets opened and here is place to "bind" your designer to the control properties */
    refreshUI: function () {
        var controlData = this._propertyEditor.get_control().Settings; /* JavaScript clone of your control - all the control properties will be properties of the controlData too */

        /* RefreshUI Message */
        jQuery(this.get_pagename()).val(controlData.PageName);
        jQuery(this.get_pageurl()).val(controlData.PageURL);
    },

    /* Called when the "Save" button is clicked. Here you can transfer the settings from the designer to the control */
    applyChanges: function () {
        var controlData = this._propertyEditor.get_control().Settings;

        /* ApplyChanges Message */
        controlData.PageName = jQuery(this.get_pagename()).val();
        controlData.PageURL = jQuery(this.get_pageurl()).val();
    },

    /* --------------------------------- event handlers ---------------------------------- */

    /* --------------------------------- private methods --------------------------------- */

    /* --------------------------------- properties -------------------------------------- */

    /* Message properties */
    get_pagename: function () { return this._pagename; },
    set_pagename: function (value) { this._pagename = value; },
    get_pageurl: function () { return this._pageurl; },
    set_pageurl: function (value) { this._pageurl = value; }
}

SitefinityWebApp.WidgetDesigners.RedirectionLink.RedirectionLinkDesigner.registerClass('SitefinityWebApp.WidgetDesigners.RedirectionLink.RedirectionLinkDesigner', Telerik.Sitefinity.Web.UI.ControlDesign.ControlDesignerBase);
