require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
gem "minitest"
require "minitest/autorun"

#Main Identity page====================================
class MainIdentityPage < BrowserContainer
  
  #Click on New Identity button.
  def new_Identity_click
    main_newIdentity_bt.wait_until_present
    main_newIdentity_bt.click
    @browser.wait(20)
  end
 
 #Click on Edit Identity button.
 def edit_Identity_click
    main_editIdentity_bt.wait_until_present
    main_editIdentity_bt.click
    @browser.wait(20)   
 end
 
   #Click on Delete Identity button.
  def delete_Identity_click
    main_deleteIdentity_bt.wait_until_present
    @browser.wait(5)
    main_deleteIdentity_bt.click
    @browser.wait(20)
  end
  
  #Verify the created identity is in the tree.
  def verifyIdentityInGrid(identityname)
    @browser.span(:title => identityname).exists?
  end
     
   #Click a identity
  def ClickIdentityInGrid(identityname)
    @browser.span(:title => identityname).click
  end   
     
  private 

  #New Identity button.
  def main_newIdentity_bt
    @browser.span(:id => 'NewIdentity_label')
  end
  
  #Edit Identity button.
  def main_editIdentity_bt
    @browser.span(:id => 'EditIdentity_label')
  end  
  
  #Delete Identity button.
  def main_deleteIdentity_bt
    @browser.span(:id => 'DeleteIdentity_label')
  end  
  
  
end