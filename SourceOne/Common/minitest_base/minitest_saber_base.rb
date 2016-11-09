require 'minitest'
require 'minitest/reporters'

Minitest::Reporters.use! [Minitest::Reporters::JUnitReporter.new]

#the root class for all Saber Minitest cases
class SaberTestBase<Minitest::Test
  def self.test_order
    :alpha
  end
  
  def setup
    
  end

  def teardown
    
  end
  
end


require 'ansi/code'
require 'builder'
require 'fileutils'
module Minitest
  #customization of the JUnitReporter
  module Reporters
    class JUnitReporter < BaseReporter
      private

      def filename_for(suite)
        if(@options[:filter])
          filename = "#{@options[:filter].split('/')[1]}.xml"
        else
          file_counter = 0
          filename = "TEST-#{suite.to_s[0..240].gsub(/[^a-zA-Z0-9]+/, '-')}.xml" #restrict max filename length, to be kind to filesystems
          while File.exists?(File.join(@reports_path, filename)) # restrict number of tries, to avoid infinite loops
            file_counter += 1
            filename = "TEST-#{suite}-#{file_counter}.xml"
            puts "Too many duplicate files, overwriting earlier report #{filename}" and break if file_counter >= 99
          end
        end
        File.join(@reports_path, filename)
      end
    end
  end
  
  #put the assert message into the stdout for future triage
  module Assertions
    
    ##
    # Fails unless +test+ is truthy.

    def assert test, msg = nil
      msg ||= "Failed assertion, no message given."
      self.assertions += 1
      if Proc === msg
          msg = msg.call
      end
      if test then
        puts "Passed: " + msg
        true
      else
        puts "Failed: " + msg      
        raise Minitest::Assertion, msg
      end
    end
    
    def assert_equal exp, act, msg = nil
      #msg = message(msg, "") { diff exp, act }
      msg = message(msg){"Expected: #{mu_pp exp}\n  Actual: #{mu_pp act}"}
      assert exp == act, msg
    end

  end
  
end
