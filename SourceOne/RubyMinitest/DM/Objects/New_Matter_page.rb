require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
require_relative '..\Objects\BtnIDHelper.rb'
gem "minitest"
require "minitest/autorun"

class NewMatterPage < BrowserContainer
  
  #Input matter name.
  def set_MatterName(mattername)
    matter_name_tb.wait_until_present
    matter_name_tb.set(mattername)
  end
  
  #click on OK button.
  def click_OK
    deleteSure = BtnIDHelper.new(@browser)
    deleteSure.get_Dlg_btnID("New Matter","OK")  
  end
  #click on Cancel button
  def Click_Cancel
    deleteSure = BtnIDHelper.new(@browser)
    deleteSure.get_Dlg_btnID("New Matter","Cancel")  
  end
  
  
  #Click the add user
  def click_FindUserList(username)
    @browser.span(:text =>"Add").click
    click_user(username)
    
  end
  
  

  
  #select the second user in the userlist when add a user in creating a matter
  def click_user(username)
    @browser.div(:id=>"uniqName_9_12").div(:class=>'gridxMain').div(:class=>'gridxRow gridxRowOdd').tds(:class=>"gridxCell    ")[0].click
    /divs.each do |div|
      if  div.inner_html.include?username
           div
      end
    end/
  end
  
  #select the user in the userlist when set user managment in matter properties
  def click_firstuser1
    @browser.div(:id=>"uniqName_9_23").div(:class=>'gridxMain').div(:class=>'gridxRow gridxRowSelected').tds(:class=>"gridxCell    ")[0].click
    /divs.each do |div|
      if  div.inner_html.include?username
           div
      end
    end/
  end
  #select the user in the userlist when set user managment in matter properties
  def click_seconduser1
    @browser.div(:id=>"uniqName_9_23").div(:class=>'gridxMain').div(:class=>'gridxRow gridxRowOdd').tds(:class=>"gridxCell    ")[0].click
    /divs.each do |div|
      if  div.inner_html.include?username
           div
      end
    end/
  end
  
  private
  
  #New matter name text box.
  def matter_name_tb
    @browser.text_field(:id => "input_newmatter_name")
  end
    
end