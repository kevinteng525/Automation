require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
require_relative '..\Objects\BtnIDHelper.rb'
gem "minitest"
require "minitest/autorun"

class DeleteUserPage < BrowserContainer
   
  #Click delete usesr Yes button
  def click_delete_user_Yes
    deleteSure = BtnIDHelper.new(@browser)
    deleteSure.get_Dlg_btnID("Confirmation","Yes")    
  end 
  
end