require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
require_relative '..\Objects\BtnIDHelper.rb'
gem "minitest"
require "minitest/autorun"

class SelectUserAddPage < BrowserContainer
  
  #Input user name.
  def set_UserName(username)
    user_name_tb.wait_until_present
    user_name_tb.set(username)
  end
  
  #Click find button
  def FindUser
    if @browser.div(:text=>'Find').exists?                   
          @browser.div(:text=>'Find').click
    end 
    
  end
  
  #click on OK button.
  def click_OK
    deleteSure = BtnIDHelper.new(@browser)
    deleteSure.get_Dlg_btnID("Select User","OK")     
  end
  
  private
  
  #Add user name text box.
  def user_name_tb
    @browser.text_field(:id => "addressbook_dlg_search")
  end  
    
end