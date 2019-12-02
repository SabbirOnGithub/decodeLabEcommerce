

//-----------------------------------------------------------------------------------------------
// Find which tab should be selected after load data using ajax
//-----------------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------------
// Loads a Url based on MVC Action, Controller and RouteValues in a PopUp Window
//-----------------------------------------------------------------------------------------------
function ViewPopUp(action, controller, data, type, windowName) {
	var url = controller + "/" + action;

	var windowElement = $('#' + windowName).data('tWindow');
	windowElement.center().open();

	var popContent = $('#' + windowName).find("#popupContentDiv");

	var Data = data;

	$.ajax({
		type: type,
		url: url,
		data: Data,
		success: function (html) {
			HideLoadingImage();
			window.EditMode = false;
			popContent.length > 0 && popContent.html(html);
			typeof callback == "function" && callback.call(this, html);
		},
		error: function () {
			alert("An unexpected error has occurred while trying to process your request.");
		}
	})
}

function HideLoadingImage() {
	if ($('#ajaxBusy').is(':visible')) {
		$('#ajaxBusy').hide();
	}
}



window.timeGap = new Date().getTimezoneOffset() / -60;








function AddToken(selectedToken) {
	tinyMCE.activeEditor.execCommand('mceInsertContent', false, selectedToken);
}



function UpdateEditor(html) {
	tinyMCE.activeEditor.setContent(html);
	$("#Window").data("tWindow").close();
}


//attach autocomplete 
/*Autocomplete plugin*/

(function ($) {
	
	var methods = {
		init: function (options) {
        var settings = $.extend({
            'url': '',
						'customToken':true,
            'data': ''
        }, options);
        $(this).data("settingsData", settings);

				return this.each(function(){
					$(this).wrap('<div id="friends" class="ui-helper-clearfix" />');
					$(this).autocomplete($(this).multiTokenAutoComplete("autoCompleteOptions"));
        });
    },

		destroy : function( ) {
      return this.each(function(){
        $(window).unbind('.multiTokenAutoComplete');
      })
    },

    setSettings: function (settings) {
        $(this).data("settingsData", settings);
    },


    autoCompleteOptions: function () {
        var $this = $(this);
				
				$(this).bind('keyup.multiTokenAutoComplete', methods.onKeyup);
				$(this).bind('keydown.multiTokenAutoComplete', methods.onKeydown);
				$(this).parent('div').bind('click.multiTokenAutoComplete', methods.onClick);
				
				$(this).multiTokenAutoComplete("generateInitialToken");
        $(this).multiTokenAutoComplete("setData");

				return {
						
            source:function(req, add){
							//pass request to server
							var settings = $this.data("settingsData");
							$.getJSON(settings.url, req , function (data) {
								
								//create array for response objects
								var suggestions = [];
								
								//process response
								$.each(data, function(i, val){								
									suggestions.push(val);
								});
								
								//pass array to callback
								add(suggestions);
							});
						},
            //width: 300,
            //delimiter: /(,|;)\s*/,
            deferRequestBy: 0, //miliseconds
            //params: { country: 'Yes' },
            noCache: true, //set to true, to disable caching
            //define select handler
						select: function (e, ui) {

							//create formatted friend
							var item = ui.item.value;
							$(this).multiTokenAutoComplete("addToken", item);
							ui.item.value = "";
            }
        };
    },
		
		
		//OnPress comma create token
		onKeyup: function (event) {
			var settings = $(this).data("settingsData");
			if ( settings.customToken && event.keyCode === 188 && $(this).val().length > 1 ) {
				var item = $(this).val().substring(0, $(this).val().length - 1); 
				$(this).multiTokenAutoComplete("addToken", item);
				$(this).val("");
			}
		},

		//OnPress Backspace delete token
		onKeydown: function (event) {
			if (event.keyCode === 8 && $(this).val().length === 0) {
				$(this).parent().find("span").last().remove();
				$(this).multiTokenAutoComplete("setData");
			}
		},

		onClick: function () {
				$(this).find('input').focus();
		},

		onRemove: function () {
			$target = $(this).parent().parent();
			$(this).parent().remove();
			$target.find('input').multiTokenAutoComplete("setData");
			console.log($target);
		},

		generateInitialToken: function () {
			var settings = $(this).data("settingsData");
			$(this).data("tokenData", settings);
			var data = settings.data.split(',');
			if(settings.data !== "")
			{
				for(var i=0; i < data.length; i++)
				{
					$(this).multiTokenAutoComplete("addToken", data[i]);
				}
			}
			$(this).val("");
			
    },

		getData: function () {
			return $(this).data("tokenData");
    },

    setData: function () {
			var data = $.map($(this).closest('div').find('span'), function(elem, index){
										return $(elem).text().substring(0, $(elem).text().length - 1);
									}).join(",");
      $(this).closest('div').data("tokenData", data);
    },

		addToken: function (itemValue) {
			
			var span = $("<span>").text(itemValue),
								a = $("<a>").addClass("remove").attr({
									href: "javascript:",
									title: "Remove " + itemValue
								}).text("x").appendTo(span);

			
			span.find('a.remove').bind('click.multiTokenAutoComplete', methods.onRemove);
			
			span.insertBefore("#" + $(this).attr("id"));
			$(this).multiTokenAutoComplete("setData");
		}
		
	};

	$.fn.multiTokenAutoComplete = function (method) {

		// Method calling logic
		if (methods[method]) {
			return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
		} else if (typeof method === 'object' || !method) {
			return methods.init.apply(this, arguments);
		} else {
			$.error('Method ' + method + ' does not exist on jQuery.tooltip');
		}

	};

})(jQuery);


/*Cloning plugin*/
(function ($) {
	var objCount = 0,
			objId,
			clonnedObj = 1;	
	var methods = {
		init: function (options) {
			var settings = $.extend({
          'data': ''
      }, options);
      $(this).data("settingsData", settings);
			
			return this.each(function () {
				
				var addBtn = $('<button>').text('+').attr('id','addBtn');
				var removeBtn = $('<button>').text('-').attr('id','removeBtn');
				removeBtn.appendTo($(this)); 
				addBtn.appendTo($(this)); 
				
				objId = $(this).attr('id');
				
				$(this).hide();
				$(this).find(".date").hide();

				$(this).cloneObject("generateInitialCriteria");
				
			});
			
		},
		destroy: function () {

			return this.each(function () {
				//$(window).unbind('.tooltip');
			})

		},
		setSettings: function (settings) {
        $(this).data("settingsData", settings);
    },
		
		

		generateInitialCriteria: function () {
			 var criteria = $(this).data("settingsData").data.split(",");
			 var keyWord;
			$.map( criteria, function(val, i) {
				var cloneObj = $("#criteria-holder").clone().show();
				cloneObj.insertBefore($("#criteria-holder")).attr('id', "criteria-holder" + String(i));
				
				if(i > 0)
				{
					$("#criteria-holder"+String(i-1)).find('#addBtn').remove();
				}
				
				$("#criteria-holder" + String(i)).find("#addBtn").bind('click.cloneObject', methods.onClickAdd);
				$("#criteria-holder" + String(i)).find("#removeBtn").bind('click.cloneObject', methods.onClickRemove);
				$("#criteria-holder" + String(i)).find("#Columns").bind('click.cloneObject', methods.onColumnChange);
    
		
				$("#criteria-holder" + String(i)).find("#Columns").val(val.split("^")[0])
				$("#criteria-holder" + String(i)).find("#Conditions").val(val.split("^")[1])
				
				keyWord = val.split("^")[0];
				
				$("#criteria-holder" + String(i)).find("#KeyWord").val(val.split("^")[2])

				if (keyWord.indexOf("Date") !== -1 || keyWord.indexOf("Created on") !== -1) {
					$("#criteria-holder" + String(i)).find(".date").datepicker({ dateFormat: 'yy-mm-dd' }).show();
					$("#criteria-holder" + String(i)).find(".date").val(val.split("^")[2]);
					$("#criteria-holder" + String(i)).find("#KeyWord").hide();
				}

				$("#criteria-holder" + String(i)).find("#AndOr").val(val.split("^")[3])

				clonnedObj = i+1;
			});
			
		},

		onClickAdd: function () {
				objCount = objCount + 1;
				var cloneObj = $(this).parent().clone();
				cloneObj.insertAfter($(this).parent()).attr('id', objId + String(objCount));//.attr('id', cloneObj.attr('id') + objCount);  $this.cloneObject("getObjId")
				$(this).parent().find('#addBtn').remove();
				$("#" +  objId + String(objCount)).find("#addBtn").bind('click.cloneObject', methods.onClickAdd);
				$("#" +  objId + String(objCount)).find("#removeBtn").bind('click.cloneObject', methods.onClickRemove);
				$("#" +  objId + String(objCount)).find("#Columns").bind('click.cloneObject', methods.onColumnChange);
				clonnedObj ++;
		},
		onColumnChange: function () {
		    if ($(this).val() == null) return;
				var selectedItem = $(this).val().split(".")[1];
				$(this).parent().find(".date").datepicker({ dateFormat: 'yy-mm-dd' });
				if (selectedItem.indexOf("Date") !== -1 || selectedItem === "CreatedOnUtc" || selectedItem === "LastActivityDateUtc" ||selectedItem === "LastLoginDateUtc" ) {
					$(this).parent().find(".date").show();
					$(this).parent().find("#KeyWord").hide();
				}
				else {
					$(this).parent().find(".date").datepicker("destroy");
					$(this).parent().find(".date").hide();
					$(this).parent().find("#KeyWord").show();
				}
		},
		onClickRemove: function () {
				if(clonnedObj === 1)
				{
					alert("This Item cannot be removed");
					return false;
				}
				if($(this).parent().find("#addBtn").length > 0)
				{
					$(this).parent().find("#addBtn").insertAfter($(this).parent().prev().find("#removeBtn"));
				}
				clonnedObj --;
				$(this).parent().remove();
		},
		update: function (content) {
			// ...
		}
	};

	$.fn.cloneObject = function (method) {

		if (methods[method]) {
			return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
		} else if (typeof method === 'object' || !method) {
			return methods.init.apply(this, arguments);
		} else {
			$.error('Method ' + method + ' does not exist on jQuery.cloneObject');
		}

	};

})(jQuery);