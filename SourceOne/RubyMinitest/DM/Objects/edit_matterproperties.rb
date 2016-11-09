require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
require_relative '..\Objects\BtnIDHelper.rb'
gem "minitest"
require "minitest/autorun"

class Editmatterproperties < BrowserContainer
  
  #edit matter properties search folder
  def allsearch_checkbox(foldername)
    @browser.div(:id=>"MatterSearchFolder_0").span(:class=>"gridxIndirectSelectionCheckBox dijitReset dijitInline dijitCheckBox dijitCheckBoxChecked ").click
    @browser.div(:id=>"MatterSearchFolder_0").div(:class=>"gridxMain").div(:class=>"gridxBody gridxBodyRowHoverEffect").font(:title=>foldername).click
  end
  
  
end