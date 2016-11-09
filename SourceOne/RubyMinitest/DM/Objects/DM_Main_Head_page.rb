require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'
gem "minitest"
require "minitest/autorun"

class MainHeadPage < BrowserContainer
  
  #Click on matter link.
  def clickMatterLink
    matter_link.wait_until_present
    matter_link.click
    @browser.wait(20)
  end
  
  
  
    #matter summary page
  def mattersummary
      sleep(2)
      @browser.span(:id =>"matter-summary-title").text.include?"Personal Matter"
  end 
  private
  
  #Matters head button.
  def matter_link
    @browser.a(:id => 'header_link_matters')
  end
  
  #Identity head button.
  
  #Administration head button.
  
  
end