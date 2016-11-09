require 'watir-webdriver'
require_relative '..\Objects\DM_Objects.rb'
require_relative '..\Variables\Variables_Configure.rb'
gem "minitest"
require "minitest/autorun"

class BtnIDHelper < BrowserContainer  
  
   #get dialog's button ID. Such as the delete search confirmation dialog. Its dlg_name is Confirmation and the btn_text is OK.
   #if two dialogs name are same,it may doesn't work.
   def get_Dlg_btnID (dlg_name, btn_text)
         okButton = dlg_Ok_button(dlg_Parent_Div(dlg_name),btn_text)
         okButton.wait_until_present
         okButton.click
         sleep(5)
         @browser.wait 
   end
   
     #Keyboard enter ok.
   def enterAssignToMatter
    @browser.send_keys :tab
    @browser.send_keys :tab
    @browser.send_keys :tab
    @browser.send_keys :enter
   end
   
   #get the dlg content
   def getdlg_content(dlg_div)
     @browser.div(:id=>dlg_div).div(:class =>"messagePrompt")
   end
   
   def get_Dlg_btnID_debug (dlg_name, btn_text, id_text)
         dlg_Ok_button(dlg_Parent_Div(dlg_name),btn_text).wait_until_present
         dlg_Ok_button(dlg_Parent_Div(dlg_name),btn_text).focus()
         dlg_Ok_button_debug(dlg_Parent_Div(dlg_name),id_text).click
         sleep(5)
         @browser.wait 
   end
   
   def get_Dlg_btnID_ForId (dlg_name, btn_text,tagname)
         dlg_Ok_Text(dlg_Parent_Div(dlg_name),btn_text).wait_until_present
         dlg_Ok_Text(dlg_Parent_Div(dlg_name),btn_text).focus()
         dlg_Ok_Text(dlg_Parent_Div(dlg_name),btn_text).set(tagname)
         dlg_Ok_Text(dlg_Parent_Div(dlg_name),btn_text).focus()
         @browser.wait 
   end
   
  
   
   
   

#Dialog rootDivid.
   def dlg_Parent_Div(dlg_name)
      if @browser.span(:text => dlg_name).exists? 
         if (@browser.span(:text => dlg_name,:class=>"dijitDialogTitle").id).include?'_title'
           (@browser.span(:text => dlg_name,:class=>"dijitDialogTitle").id).gsub('_title','')
          else
           (@browser.span(:text => dlg_name,:class=>"dijitDialogTitle").id).gsub('_label','') 
         end
      else
       puts "Cannot get "+dlg_name+" dialog's "+btn_text+"button !"
      end
   end
#Dialog Ok button.
   def dlg_Ok_button(dlg_div, btn_text)
       if @browser.div(:id=>dlg_div).span(:text => btn_text).exist?
          @browser.div(:id=>dlg_div).span(:text => btn_text)
       else
          sleep(2)
          @browser.div(:id=>dlg_div).td(:text => btn_text)
       end
   end
      def dlg_Ok_button_debug(dlg_div, btn_text)
       @browser.div(:id=>dlg_div).span(:id => btn_text)
   end
#Dialog Ok button.
   def dlg_Ok_Text(dlg_div, btn_text)
       if @browser.div(:id=>dlg_div).text_field(:id => btn_text).exist?
          @browser.div(:id=>dlg_div).text_field(:id => btn_text)
       else
         sleep(5)
          @browser.div(:id=>dlg_div).text_field(:id => btn_text)
       end
       
   end
   
end