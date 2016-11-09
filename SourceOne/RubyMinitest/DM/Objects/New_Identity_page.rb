require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
require_relative '..\Objects\BtnIDHelper.rb'
gem "minitest"
require "minitest/autorun"

class NewIdentityPage < BrowserContainer
   
  #Input identity name.
  def set_IdentityName(identityname)
    identity_name_tb.wait_until_present
    identity_name_tb.set(identityname)
  end
  
  #click on Save and Close button.
  def click_Save_Close
    deleteSure = BtnIDHelper.new(@browser)
    deleteSure.get_Dlg_btnID("New Identity","Save and Close")
    sleep(5)
  end
  
  private
  
  
  #New identity name text box.
  def identity_name_tb
    @browser.text_field(:id => "input_identity_name")
  end 
  
end