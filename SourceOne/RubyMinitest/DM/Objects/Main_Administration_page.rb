require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
gem "minitest"
require "minitest/autorun"

#Main Administration page====================================
class MainAdministrationPage < BrowserContainer
  
  #Click on User Manager button.
  def user_manager_click
    main_UserManager_bt.wait_until_present
    main_UserManager_bt.click
    @browser.wait(20)
  end
  
  #Click on Configuration button.
  def configuration_click
    main_Configuration_bt.wait_until_present
    main_Configuration_bt.click
    @browser.wait(20)
  end
  
  #Click Add User... button
  def add_user_click
    main_AddUser_bt.wait_until_present
    main_AddUser_bt.click
    @browser.wait(20)
  end
  
  #click add personnal matter button
  def add_personalmatter_click
    main_addpersonalmatter_bt.wait_until_present
    main_addpersonalmatter_bt.click
  end
  
  #click remove personnal matter button
  def remove_personalmatter_click
    main_removepersonalmatter_bt.wait_until_present
    main_removepersonalmatter_bt.click
  end
 
 #Click check all permission
  def checkall_click
    check_UserPermission.wait_until_present
    check_UserPermission.click
    @browser.wait(20)
  end
  
  #Click the user in the user list 
  def click_MainUserList
    if @browser.span(:title => $deleteusername).exists?
      @browser.span(:title => $deleteusername).click
    else
      @browser.span(:title => $deleteUserNameO).click
    end
  end
  
  #Click Remove User button
  def remove_user_click
    main_RemoveUser_bt.wait_until_present
    main_RemoveUser_bt.click
    @browser.wait(20)
  end
  
  #get the first user name in the grid
  def get_firstuser_personalmatter_status
    first_user_status.inner_html.split("\"")[1].split()[0]
  end
  
  #click the first user name in the grid
  def click_firstuser_personalmatter
    first_user_status.wait_until_present
    first_user_status.click
  end
  
  #get the first user name in the grid
  def  get_first_user
    first_user.inner_html.split("\"")[1].split()[0]
  end
  
  
  #Verify the element is in the administration page.
  def verifyElementExist(element)
    @browser.span(:text => element).exists?
  end
  
  #Verify the add user is in the grid.
  def verifyUserInGrid(deleteusername)
    @browser.span(:title => deleteusername).exists?
  end
  
  #click the second user
  def click_seconduser
      second_user.wait_until_present
      second_user.click
 end
 
 def click_removeuser
   main_RemoveUser_bt.wait_until_present
   main_RemoveUser_bt.click
 end
  
  
  #select the user 
  def  user_select_click(username)
    @browser.span(:title=>username).click
  end
  
  
  
  #find the mattername in the all matters grid
 def findmatter_in_allmatters(matter)
   @browser.span(:title=>matter)
 end
  #all matters button
  def allmatters_bt
    @browser.span(:id=>"AdminTabs_tablist_AdminTab_AllMatters")
  end
  
  private 

  #User Manager button.
  def main_UserManager_bt
    @browser.span(:id => 'AdminTabs_tablist_AdminTab_UserManager')
  end 
  
  #the first user  in the usermanger
  def first_user
    @browser.div(:class=>'gridxMain').td(:class=>'gridxCell')
  end
  
  #the first user  in the usermanger
  def second_user
    @browser.div(:class=>'gridxMain').div(:class=>'gridxRow gridxRowOdd').td(:class=>'gridxCell')
  end
  
  def first_user_status
    @browser.div(:class=>'gridxMain').tds(:class=>'gridxCell')[1]
  end
  
  #Configuration button.
  def main_Configuration_bt
    @browser.span(:id => 'AdminTabs_tablist_AdminTab_Configuration')
  end  
  
  
  #add personal matter button
  def main_removepersonalmatter_bt
    @browser.span(:id => 'dijit_form_Button_6_label')
  end
  #add personal matter button
  def main_addpersonalmatter_bt
    @browser.span(:id => 'dijit_form_Button_5_label')
  end 
  
  #Add User... button. This ID is the only.
  def main_AddUser_bt
    @browser.span(:id => 'dijit_form_Button_3_label')
  end
  

  
  #Remove User button. This ID is the only.
  def main_RemoveUser_bt
    @browser.span(:id => 'dijit_form_Button_4_label')
  end
  
  #Check user's permission.
  def check_UserPermission
    @browser.div(:class => 'gridxRowHeaderHeader').span(:class =>'gridxIndirectSelectionCheckBox dijitReset dijitInline dijitCheckBox')
  end
  
  
end