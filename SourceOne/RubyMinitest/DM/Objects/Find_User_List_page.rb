require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
require_relative '..\Objects\BtnIDHelper.rb'
gem "minitest"
require "minitest/autorun"

class FindUserListPage < BrowserContainer
  
  #Click the add user
  def click_FindUserList(username)
    @browser.span(:title => username).click
    #click_user(username)
    #@dmweb.btn_id_helper.get_Dlg_btnID("Users","OK")
  end
  

  
  #click on OK button.
  def click_OK
    deleteSure = BtnIDHelper.new(@browser)
    deleteSure.get_Dlg_btnID("User List","OK")       
  end 
 
end