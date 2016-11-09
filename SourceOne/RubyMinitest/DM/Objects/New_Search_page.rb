require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
require_relative '..\Objects\BtnIDHelper.rb'
gem "minitest"
require "minitest/autorun"

class NewSearchPage < BrowserContainer
  
  #Input search name.
  def set_SearchName(searchname)
    search_name_tb.wait_until_present
    search_name_tb.set(searchname)
  end
  
   #Input Folder name.
  def set_FolderName(searchname)
    folder_name_tb.wait_until_present
    folder_name_tb.set(searchname)
  end
  
  def tab_to_searchmenu
     @browser.send_keys :tab
     @browser.send_keys :enter
     @browser.send_keys :down
     @browser.send_keys :down
     @browser.send_keys :down
     @browser.send_keys :down
     @browser.send_keys :down
     @browser.send_keys :enter
  end
  
  
  #click on search OK button.
  def searchclick_OK
    deleteSure = BtnIDHelper.new(@browser)
    deleteSure.get_Dlg_btnID("New Search","OK")    
  end
  
   #click on folder OK button.
  def folderclick_OK
    deleteSure = BtnIDHelper.new(@browser)
    deleteSure.get_Dlg_btnID("New Folder","OK")    
  end
  
  
  #click on OK button.
  def click_Cancel
    deleteSure = BtnIDHelper.new(@browser)
    deleteSure.get_Dlg_btnID("New Search","Cancel")    
  end
  
  
  private
  
  #New search name text box.
  def search_name_tb
    @browser.text_field(:id => "new_search_name")
  end
  
  #New Folder name text box.
  def folder_name_tb
    @browser.text_field(:id => "new_folder_name")
  end
  
end
