require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
gem "minitest"
require "minitest/autorun"

#Main Matter page====================================
class AllItemsPage < BrowserContainer
  
  #verify the total number in all items is correct.
  def verifyTotalInAllItems(total)
    total_selected_text.text.include? total
  end
  #very the matter name  is correct
  def verifyMatter(name)
    @browser.span(:id=>"matter-summary-title").text.include? name
  end
  
 #find the search folder
 def searchfolder(folder) 
   @browser.font(:title=>folder)
 end
  
  #find the matterstate
  def matterstate(matter)
    @browser.span(:id=>"matter-summary-state")
  end
  
  #Click Assign tag button.
  def clickAssign(menu)
    search_assign_bt(menu).wait_until_present
    search_assign_bt(menu).click
    sleep(1)
  end
  #assign the items tag
  def enterAssignToTags
    @browser.send_keys :right
    @browser.send_keys :left
    @browser.send_keys :down
    @browser.send_keys :enter
  end
  
  #get the tag name
  def get_tag
    @browser.span(:class=>'tagIcon').attribute_value 'title'
  end
  
  
  private
  
  #this is the "Total: xxx; Selected: xxx" area in the result page.
  def total_selected_text
    @browser.div(:class => 'gridxSummary')
  end
  
  
  #Assign button tag.
  def search_assign_bt(menu)
    @browser.span(:text => menu)
  end
  
  
end