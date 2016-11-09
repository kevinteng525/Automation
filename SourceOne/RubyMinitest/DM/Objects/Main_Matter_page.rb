require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
gem "minitest"
require "minitest/autorun"

#Main Matter page====================================
class MainMatterPage < BrowserContainer
  
  #Click on New Matter button.
  def new_matter_click
    main_newMatter_bt.wait_until_present
    main_newMatter_bt.click
    @browser.wait(20)
  end
 
 #Click on New Search button.
 def new_search_click
    main_newSearch_bt.wait_until_present
    main_newSearch_bt.click
    @browser.wait(20)   
 end
 
 #Click on Delete Search button.
 def delete_search_click
    main_deleteSearch_bt.wait_until_present
    main_deleteSearch_bt.click
    @browser.wait(20)   
 end
 
 
 
  #Click matter properties.
 def matter_properties_click
    matter_properties_bt.wait_until_present
    matter_properties_bt.click
    @browser.wait(20)   
 end
 
 #Click on New Folder button.
 def new_Folder_click
    main_newFolder_bt.wait_until_present
    main_newFolder_bt.click
    @browser.wait(20)   
 end
 
 #Click on Delete Matter button.
 def delete_matter_click
    main_deleteMatter_bt.wait_until_present
    main_deleteMatter_bt.click
    @browser.wait(20)   
 end
 
 
 
  #get the staff grid in the matter summary,check the user list
  def staff_grid(username)
      @browser.div(:id=>"matter-summary-staff-grid").div(:class=>'gridxMain').div(:class=>'gridxRow').tds(:class=>"gridxCell    ")[0].inner_html.include?username
  end
 
  private 

  #New Matter button.
  def main_newMatter_bt
    @browser.span(:id => 'NewMatter_label')
  end
  
  #New Search button.
  def main_newSearch_bt
    @browser.span(:id => 'NewSearch_label')
  end 
  
  #Delete Search button
  def main_deleteSearch_bt
    @browser.span(:id => 'DeleteSearch_label')
  end 
  
  #matter properties button.
  def matter_properties_bt
    @browser.span(:id => 'MatterProperty_label')
  end 
  
  #New Folder button.
  def main_newFolder_bt
    @browser.span(:id => 'NewFolder_label')
  end
   
  
  #Delete Matter button.
  def main_deleteMatter_bt
    @browser.span(:id =>'DeleteMatter')
  end
  
end

#User Preferences page.
class UserAccountPage < BrowserContainer
  #Verify whether login is successfully.
  def logged_in(username)
    logged_in_element.text.include? username
  end

  private

  #User Preferences icon.
  def logged_in_element
    @browser.div(:id => "loginStatus")
  end
end 

#User menu bar page. Include Matters, Identities, Administration.
class UserMenuPage < BrowserContainer
  #Click the Identities to change page.
  def ClickIdentities
    identities_menu.wait_until_present
    identities_menu.click
    @browser.wait(20)
  end
  
  #Click the Administration to change page.
  def clickAdministration
    administration_menu.wait_until_present
    administration_menu.click
    @browser.wait(20)
  end
  
  #Verify the page has identities.
  def verifyIdentityPage
    @browser.link(:id => "header_link_identity").exists?
  end
  
  
 
  
  
    
  
  
  #Identities menu bar
  def identities_menu
    @browser.link(:id => "header_link_identity")
  end
  
  #Administration menu bar
  def administration_menu
    @browser.link(:id => "header_link_admin")
  end
end