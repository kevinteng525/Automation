require File.expand_path('../boot', __FILE__)

require 'rails/all'

# Require the gems listed in Gemfile, including any gems
# you've limited to :test, :development, or :production.
Bundler.require(:default, Rails.env)

module Galaxy
  class Application < Rails::Application
    #Config the js and css compression
	config.assets.precompile << Proc.new do |path|
      if path =~ /\.(css|js|scss)\z/
        full_path = Rails.application.assets.resolve(path).to_path
        app_assets_path1 = Rails.root.join('app', 'assets').to_path
        app_assets_path2 = Rails.root.join('public', 'assets').to_path
        #app_assets_path3 = Rails.root.join('vendor', 'assets').to_path

        if full_path.starts_with? app_assets_path1
          true
        else
          if full_path.starts_with? app_assets_path2
            true
          else
            #if full_path.starts_with? app_assets_path3
            #  true
            #else
              false
            #end
          end
        end
      end
    end
	# Settings in config/environments/* take precedence over those specified here.
    # Application configuration should go into files in config/initializers
    # -- all .rb files in that directory are automatically loaded.

    # Set Time.zone default to the specified zone and make Active Record auto-convert to this zone.
    # Run "rake -D time" for a list of tasks for finding time zone names. Default is UTC.
    # config.time_zone = 'Central Time (US & Canada)'

    # The default locale is :en and all translations from config/locales/*.rb,yml are auto loaded.
    # config.i18n.load_path += Dir[Rails.root.join('my', 'locales', '*.{rb,yml}').to_s]
    # config.i18n.default_locale = :de
    FORCE_SERVER = '10.98.28.190:8080'
  end
end
