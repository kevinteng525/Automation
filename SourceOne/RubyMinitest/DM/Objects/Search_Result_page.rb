require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
gem "minitest"
require "minitest/autorun"


class SearchResultPage < BrowserContainer
  
   #click on the search Find button.
  def clickSearchFindButton
    search_find_bt.wait_until_present
    search_find_bt.click
    @browser.wait(20)
  end
  
  #check search all checkbox.
  def checkAllResult
    sleep(2)
    search_all_checkbox.wait_until_present
    search_all_checkbox.click
    @browser.wait(20)
  end
  
  #Click Assign button.
  def clickAssign(menu)
    search_assign_bt(menu).wait_until_present
    search_assign_bt(menu).click
    sleep(1)
  end
  
  #Click Assign to matter icon.
  def clickAssignToMatter
    search_assignToMatter_bt.wait_until_present
    search_assignToMatter_bt.click
    @browser.wait(20)  
  end
  
  #enter on Assign button.
  def enterAssignToMatter
    @browser.send_keys :right
    @browser.send_keys :left
    @browser.send_keys :enter
  end
  
  #click Export button.
  def clickExport
    search_export_bt.wait_until_present
    search_export_bt.click
    @browser.wait(20)
  end
  
  #enter Export.
  def enterExport
    @browser.send_keys :right
    @browser.send_keys :left
    @browser.send_keys :enter
  end

  
  #Find button.
  def search_find_bt
    @browser.div(:text => 'Find')
  end
  
  #CheckBox All.
  def search_all_checkbox
    @browser.span(:class => 'gridxIndirectSelectionCheckBox dijitReset dijitInline dijitCheckBox')
  end
  
   #checkbox for each
  def search_each(x)
    @browser.spans(:class=> 'gridxIndirectSelectionCheckBox dijitReset dijitInline dijitCheckBox')[x]
  end
  
  #Assign button.
  def search_assign_bt(menu)
    @browser.span(:text => menu)
  end
  
  #Assign to matter icon.
  def search_assignToMatter_bt
    @browser.div(:class => 'dijitPopup dijitMenuPopup').table(:class =>'dijit dijitReset dijitMenuTable dijitMenu dijitMenuPassive').tbody(:class => 'dijitReset').tr(:class => 'dijitReset dijitMenuItem')
  end
  
  #Export button.
  def search_export_bt
    @browser.div(:id=>'dijit_PopupMenuBarItem_0').span(:text => 'Export')
  end
  
end