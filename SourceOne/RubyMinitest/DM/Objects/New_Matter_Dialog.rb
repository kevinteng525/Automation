require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
require_relative '..\Objects\BtnIDHelper.rb'
gem "minitest"
require "minitest/autorun"

class NewMatterDialog < BrowserContainer
   
  #set mattername.
  def set_MatterName(mattetrname)
    matter_name_tb.wait_until_present
    matter_name_tb.set(mattetrname)
  end
  
  #set matter description.
  def set_Matterdes(mattetrname)
    matter_des_tb.wait_until_present
    matter_des_tb.set(mattetrname)
  end
  
  #click matter hold folder button.
  def set_Matter_holdfolder
    matter_holdfolder_bt.wait_until_present
    matter_holdfolder_bt.click
  end
  
  #select the hold folder.
  def select_holdfolder(holdfolder)
    selectholdfolder = BtnIDHelper.new(@browser)
    selectholdfolder.get_Dlg_btnID("Hold Folders",holdfolder)
    selectholdfolder.get_Dlg_btnID("Hold Folders","OK")
    sleep(5)
  end
  
  
  #remove the staff
  def remove_staff(user)
    removestaff = BtnIDHelper.new(@browser)
    fulluser=user+' ('+user+'@'+$_UserDomain+')'
    removestaff.get_Dlg_btnID("New Matter",fulluser.capitalize)
    removestaff.get_Dlg_btnID("New Matter","Remove")
    removestaff.get_Dlg_btnID("User Remove","Yes")
    sleep(5)
    
  end
  
  #add the staff
  def add_staff(user)
    addstaff = BtnIDHelper.new(@browser)
    addstaff.get_Dlg_btnID("New Matter","Add")
    fulluser=user+' ('+user+'@'+$_UserDomain+')'
    addstaff.get_Dlg_btnID("Users",fulluser.capitalize)
    addstaff.get_Dlg_btnID("Users","OK")
    sleep(5)
    
  end
  
  #if the matter has only one user,remove the user will pop up a error dialog
  def error_popup
    errorpopup= BtnIDHelper.new(@browser)
    errorpopup.get_Dlg_btnID("Error","OK")
  end
  
  
  #click the newmatter ok to create matter
  def click_newmatter_OK_bt
    errorpopup= BtnIDHelper.new(@browser)
    errorpopup.get_Dlg_btnID("New Matter","OK")
  end
  
  
 private
 #New matter Hold Folder browser button.
  def matter_holdfolder_bt
     @browser.span(:id => "dijit_form_Button_4_label")
  end 
  
   #New matter description text box.
  def matter_des_tb
     @browser.text_field(:id => "input_newmatter_description")
  end 
  
  #New matter name text box.
  def matter_name_tb
     @browser.text_field(:id => "input_newmatter_name")
  end 
  
end